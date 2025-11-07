using applications.DTOs.Request.AI;
using applications.DTOs.Response.AI;
using Microsoft.Extensions.Logging;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System.Text.Json;
using applications;

namespace services.Services
{
    public class AiIntegrationService : IAiIntegrationService
    {
        private readonly IAiService _aiService;
        private readonly ILessonPlanRepository _lessonPlanRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IAiRequestRepository _aiRequestRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWordDocumentService _wordDocumentService;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;
        private readonly ILogger<AiIntegrationService> _logger;

        public AiIntegrationService(
            IAiService aiService,
            ILessonPlanRepository lessonPlanRepository,
            ILessonRepository lessonRepository,
            IQuestionRepository questionRepository,
            IQuizRepository quizRepository,
            IAiRequestRepository aiRequestRepository,
            ICurrentUserService currentUserService,
            IWordDocumentService wordDocumentService,
            ICloudinaryStorageService cloudinaryStorageService,
            ILogger<AiIntegrationService> logger)
        {
            _aiService = aiService;
            _lessonPlanRepository = lessonPlanRepository;
            _lessonRepository = lessonRepository;
            _questionRepository = questionRepository;
            _quizRepository = quizRepository;
            _aiRequestRepository = aiRequestRepository;
            _currentUserService = currentUserService;
            _wordDocumentService = wordDocumentService;
            _cloudinaryStorageService = cloudinaryStorageService;
            _logger = logger;
        }

        public async Task<LessonPlan> GenerateAndSaveLessonPlanAsync(
            AiLessonPlanRequestDto request, 
            CancellationToken cancellationToken = default)
        {
            AiRequest aiRequest = null;
            try
            {
                _logger.LogInformation("Generating lesson plan using AI for topic: {Topic}", request.Topic);

                var userId = _currentUserService.GetUserId() ?? request.TeacherId;
                if (userId == 0)
                {
                    throw new UnauthorizedAccessException("User must be authenticated to generate lesson plan");
                }

                // Create AiRequest with Pending status
                aiRequest = new AiRequest
                {
                    UserId = userId,
                    LevelId = request.LevelId,
                    RequestType = RequestType.GenerateLessonPlan,
                    Prompt = JsonSerializer.Serialize(request),
                    Status = AiRequestStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                await _aiRequestRepository.AddAsync(aiRequest);

                // Call AI service to generate lesson plan
                var aiResponse = await _aiService.GenerateLessonPlanAsync(request, cancellationToken);

                // Update AiRequest with response
                aiRequest.Response = JsonSerializer.Serialize(aiResponse);
                aiRequest.Status = AiRequestStatus.Success;
                await _aiRequestRepository.UpdateAsync(aiRequest);

                // Create LessonPlan entity
                var lessonPlan = new LessonPlan
                {
                    TeacherId = request.TeacherId,
                    LevelId = request.LevelId,
                    Grade = request.Grade,
                    Title = aiResponse.Topic,
                    Topic = aiResponse.Topic,
                    LearningObjectives = string.Join("\n", aiResponse.Objectives),
                    Duration = aiResponse.Duration,
                    Content = aiResponse.Assessment,
                    AiPrompt = JsonSerializer.Serialize(request),
                    AiResponse = JsonSerializer.Serialize(aiResponse),
                    AiRequestId = aiRequest.AIRequestId,
                    Status = LessonPlanStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    Version = 1,
                    IsPublic = false
                };

                // Save LessonPlan first to get the ID
                await _lessonPlanRepository.AddLessonPlanAsync(lessonPlan, cancellationToken);

                // Create Lesson entities from sections
                var lessons = new List<Lesson>();
                int order = 1;

                foreach (var section in aiResponse.Sections)
                {
                    var lesson = new Lesson
                    {
                        LessonPlanId = lessonPlan.LessonPlanId,
                        Title = section.Title,
                        Objective = string.Join("\n", section.Activities),
                        Content = section.Content,
                        Order = order++,
                        IsGeneratedByAI = true,
                        IsShared = false,
                        CreatedAt = DateTime.UtcNow,
                        LessonDetails = new List<LessonDetail>()
                    };

                    // Create LessonDetails for each activity
                    int detailOrder = 1;
                    foreach (var activity in section.Activities)
                    {
                        var lessonDetail = new LessonDetail
                        {
                            LessonId = lesson.LessonId,
                            Order = detailOrder++,
                            ContentType = ContentType.Text,
                            Content = activity,
                            CreatedAt = DateTime.UtcNow
                        };
                        lesson.LessonDetails.Add(lessonDetail);
                    }

                    lessons.Add(lesson);
                }

                // Save all lessons
                foreach (var lesson in lessons)
                {
                    await _lessonRepository.AddLessonAsync(lesson);
                }

                // Load lessons into lessonPlan for Word document generation
                lessonPlan.Lessons = lessons;

                // Generate Word document and upload to Cloudinary
                try
                {
                    _logger.LogInformation("Generating Word document for lesson plan: {Title}", lessonPlan.Title);
                    
                    using var wordStream = await _wordDocumentService.GenerateLessonPlanWithLessonsDocumentAsync(lessonPlan);
                    
                    var fileName = $"LessonPlan_{lessonPlan.LessonPlanId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.docx";
                    var fileUrl = await _cloudinaryStorageService.UploadWordDocumentAsync(wordStream, fileName);
                    
                    // Update lesson plan with export path
                    lessonPlan.ExportPath = fileUrl;
                    await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan, cancellationToken);
                    
                    _logger.LogInformation("Word document uploaded successfully: {Url}", fileUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to generate/upload Word document, but lesson plan was created successfully");
                    // Don't throw - lesson plan creation was successful even if document generation failed
                }

                _logger.LogInformation("Successfully created lesson plan with {LessonCount} lessons", lessons.Count);

                return lessonPlan;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating and saving lesson plan");
                
                // Update AiRequest status to Failed if it was created
                if (aiRequest != null)
                {
                    aiRequest.Status = AiRequestStatus.Failed;
                    aiRequest.Response = JsonSerializer.Serialize(new { error = ex.Message });
                    await _aiRequestRepository.UpdateAsync(aiRequest);
                }
                
                throw new Exception($"Failed to generate and save lesson plan: {ex.Message}", ex);
            }
        }

        public async Task<List<Question>> GenerateAndSaveQuestionsAsync(
            AiQuestionRequestDto request, 
            CancellationToken cancellationToken = default)
        {
            AiRequest aiRequest = null;
            try
            {
                _logger.LogInformation("Generating {Count} questions using AI for topic: {Topic}", 
                    request.Count, request.Topic);

                var currentUserId = _currentUserService.GetUserId();
                if (!currentUserId.HasValue)
                {
                    throw new UnauthorizedAccessException("User must be authenticated to generate questions");
                }

                // Create AiRequest with Pending status
                aiRequest = new AiRequest
                {
                    UserId = currentUserId.Value,
                    LevelId = request.LevelId ?? MapGradeLevelToLevelId(request.GradeLevel),
                    RequestType = RequestType.GenerateQuestions,
                    Prompt = JsonSerializer.Serialize(request),
                    Status = AiRequestStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                await _aiRequestRepository.AddAsync(aiRequest);

                // Call AI service to generate questions
                var aiResponse = await _aiService.GenerateQuestionsAsync(request, cancellationToken);

                // Update AiRequest with response
                aiRequest.Response = JsonSerializer.Serialize(aiResponse);
                aiRequest.Status = AiRequestStatus.Success;
                await _aiRequestRepository.UpdateAsync(aiRequest);

                var questions = new List<Question>();

                foreach (var aiQuestion in aiResponse.Questions)
                {
                    // Map AI question type to entity enum
                    var questionType = MapQuestionType(aiQuestion.QuestionType);

                    var question = new Question
                    {
                        QuestionBankId = request.QuestionBankId,
                        Topic = MapTopic(aiResponse.Topic),
                        QuestionText = aiQuestion.QuestionText,
                        QuestionType = questionType,
                        CorrectAnswer = aiQuestion.CorrectAnswer,
                        Explanation = aiQuestion.Solution,
                        Tags = string.Join(",", aiQuestion.Tags),
                        IsAIGenerated = true,
                        AiRequestId = aiRequest.AIRequestId,
                        Status = QuestionStatus.PendingReview,
                        CreatedAt = DateTime.UtcNow,
                        DifficultyId = request.DifficultyId,
                        Answers = new List<Answer>()
                    };

                    // Add choices as Answer entities for multiple choice questions
                    if (aiQuestion.Choices != null && aiQuestion.Choices.Any())
                    {
                        foreach (var choice in aiQuestion.Choices)
                        {
                            var answer = new Answer
                            {
                                QuestionId = question.QuestionId,
                                AnswerText = choice.Text,
                                IsCorrect = choice.IsCorrect
                            };
                            question.Answers.Add(answer);
                        }
                    }

                    questions.Add(question);
                }

                // Save all questions
                foreach (var question in questions)
                {
                    await _questionRepository.AddQuestionAsync(question);
                }

                _logger.LogInformation("Successfully created {Count} questions", questions.Count);

                return questions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating and saving questions");
                
                // Update AiRequest status to Failed if it was created
                if (aiRequest != null)
                {
                    aiRequest.Status = AiRequestStatus.Failed;
                    aiRequest.Response = JsonSerializer.Serialize(new { error = ex.Message });
                    await _aiRequestRepository.UpdateAsync(aiRequest);
                }
                
                throw new Exception($"Failed to generate and save questions: {ex.Message}", ex);
            }
        }

        public async Task<Quiz> GenerateAndSaveQuizAsync(
            AiQuizRequestDto request, 
            CancellationToken cancellationToken = default)
        {
            AiRequest aiRequest = null;
            try
            {
                _logger.LogInformation("Generating quiz using AI: {Title}", request.Title);

                var userId = _currentUserService.GetUserId() ?? request.TeacherId ?? 0;
                if (userId == 0)
                {
                    throw new UnauthorizedAccessException("User must be authenticated to generate quiz");
                }

                // Create AiRequest with Pending status
                aiRequest = new AiRequest
                {
                    UserId = userId,
                    LevelId = MapGradeLevelToLevelId(request.GradeLevel),
                    RequestType = RequestType.GenerateQuiz,
                    Prompt = JsonSerializer.Serialize(request),
                    Status = AiRequestStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                await _aiRequestRepository.AddAsync(aiRequest);

                // Call AI service to generate quiz
                var aiResponse = await _aiService.GenerateQuizAsync(request, cancellationToken);

                // Update AiRequest with response
                aiRequest.Response = JsonSerializer.Serialize(aiResponse);
                aiRequest.Status = AiRequestStatus.Success;
                await _aiRequestRepository.UpdateAsync(aiRequest);

                // Create Quiz entity
                var quiz = new Quiz
                {
                    TeacherId = request.TeacherId ?? 0,
                    LevelId = MapGradeLevelToLevelId(request.GradeLevel),
                    Title = aiResponse.Title,
                    TimeLimit = aiResponse.Duration,
                    AttemptLimit = 3, // Default value
                    TotalScore = aiResponse.TotalPoints,
                    IsAIGenerated = true,
                    Status = QuizStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    Questions = new List<Question>()
                };

                // Create Question entities from quiz questions
                foreach (var aiQuestion in aiResponse.Questions)
                {
                    var questionType = MapQuestionType(aiQuestion.QuestionType);

                    var question = new Question
                    {
                        QuizId = quiz.QuizId,
                        QuestionBankId = request.QuestionBankId,
                        Topic = MapTopic(aiResponse.Topic),
                        QuestionText = aiQuestion.QuestionText,
                        QuestionType = questionType,
                        CorrectAnswer = aiQuestion.CorrectAnswer,
                        Explanation = aiQuestion.Solution,
                        Tags = string.Join(",", aiQuestion.Tags),
                        IsAIGenerated = true,
                        AiRequestId = aiRequest.AIRequestId,
                        Status = QuestionStatus.Approved, // Auto-approve for quizzes
                        CreatedAt = DateTime.UtcNow,
                        Answers = new List<Answer>()
                    };

                    // Add choices as Answer entities
                    if (aiQuestion.Choices != null && aiQuestion.Choices.Any())
                    {
                        foreach (var choice in aiQuestion.Choices)
                        {
                            var answer = new Answer
                            {
                                QuestionId = question.QuestionId,
                                AnswerText = choice.Text,
                                IsCorrect = choice.IsCorrect
                            };
                            question.Answers.Add(answer);
                        }
                    }

                    quiz.Questions.Add(question);
                }

                await _quizRepository.AddAsync(quiz);

                _logger.LogInformation("Successfully created quiz with {QuestionCount} questions", quiz.Questions.Count);

                return quiz;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating and saving quiz");
                
                // Update AiRequest status to Failed if it was created
                if (aiRequest != null)
                {
                    aiRequest.Status = AiRequestStatus.Failed;
                    aiRequest.Response = JsonSerializer.Serialize(new { error = ex.Message });
                    await _aiRequestRepository.UpdateAsync(aiRequest);
                }
                
                throw new Exception($"Failed to generate and save quiz: {ex.Message}", ex);
            }
        }

        #region Helper Methods

        private QuestionType MapQuestionType(string aiQuestionType)
        {
            return aiQuestionType.ToLower() switch
            {
                "multiple_choice" => QuestionType.MultipleChoice,
                "true_false" => QuestionType.TrueFalse,
                "short_answer" => QuestionType.ShortAnswer,
                "essay" => QuestionType.Essay,
                _ => QuestionType.ShortAnswer
            };
        }

        private Topic MapTopic(string topicString)
        {
            // Simple mapping - you may want to make this more sophisticated
            if (topicString.Contains("Giải tích") || topicString.Contains("Calculus"))
                return Topic.Calculus;
            if (topicString.Contains("Lượng giác") || topicString.Contains("Trigonometry"))
                return Topic.Trigonometry;
            if (topicString.Contains("Hình học") || topicString.Contains("Geometry"))
                return Topic.Geometry;
            if (topicString.Contains("Đại số") || topicString.Contains("Algebra"))
                return Topic.Algebra;
            if (topicString.Contains("Thống kê") || topicString.Contains("Statistics"))
                return Topic.Statistics;
            if (topicString.Contains("Xác suất") || topicString.Contains("Probability"))
                return Topic.Probability;

            return Topic.Algebra; // Default
        }

        private int MapGradeLevelToLevelId(int grade)
        {
            // Map grade to level
            // 1-5: Primary (LevelId = 1)
            // 6-9: Secondary (LevelId = 2)
            // 10-12: High School (LevelId = 3)
            if (grade >= 1 && grade <= 5) return 1;
            if (grade >= 6 && grade <= 9) return 2;
            if (grade >= 10 && grade <= 12) return 3;

            return 2; // Default to secondary
        }

        #endregion

        #region AI Request History

        public async Task<IEnumerable<AiRequestHistoryDto>> GetRequestHistoryAsync(
            int userId,
            RequestType? requestType = null,
            AiRequestStatus? status = null,
            string? search = null,
            int page = 1,
            int limit = 20,
            CancellationToken cancellationToken = default)
        {
            var requests = await _aiRequestRepository.GetRequestHistoryAsync(
                userId, requestType, status, search, page, limit);

            return requests.Select(r => new AiRequestHistoryDto
            {
                RequestId = r.AIRequestId,
                UserId = r.UserId,
                UserName = r.User?.Username ?? "Unknown",
                RequestType = r.RequestType.ToString(),
                Prompt = r.Prompt,
                Response = r.Response,
                Status = r.Status.ToString(),
                Cost = r.Cost,
                CreatedAt = r.CreatedAt,
                LevelId = r.LevelId,
                LevelName = r.Level?.LevelName ?? "Unknown"
            });
        }

        public async Task<int> GetRequestCountAsync(
            int userId,
            RequestType? requestType = null,
            AiRequestStatus? status = null,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            return await _aiRequestRepository.GetTotalCountAsync(userId, requestType, status, search);
        }

        public async Task<AiRequestDetailDto?> GetRequestDetailsAsync(int requestId, CancellationToken cancellationToken = default)
        {
            var request = await _aiRequestRepository.GetByIdAsync(requestId);
            if (request == null) return null;

            return new AiRequestDetailDto
            {
                RequestId = request.AIRequestId,
                UserId = request.UserId,
                UserName = request.User?.Username ?? "Unknown",
                RequestType = request.RequestType.ToString(),
                Prompt = request.Prompt,
                Response = request.Response,
                Status = request.Status.ToString(),
                Cost = request.Cost,
                CreatedAt = request.CreatedAt,
                LevelId = request.LevelId,
                LevelName = request.Level?.LevelName ?? "Unknown",
                LessonPlanIds = request.LessonPlans?.Select(lp => lp.LessonPlanId).ToList(),
                QuestionIds = request.Questions?.Select(q => q.QuestionId).ToList()
            };
        }

        #endregion
    }
}
