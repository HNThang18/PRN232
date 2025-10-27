using applications.DTOs.Request.AI;
using Microsoft.Extensions.Logging;
using repositories.EventSourcing.Aggregates;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System.Text.Json;
using applications;
using services.EventSourcing.Projectors;

namespace services.EventSourcing
{
    public class QuizGenerationEventService : IQuizGenerationEventService
    {
        private readonly IAiService _aiService;
        private readonly IQuizRepository _quizRepository;
        private readonly IAiRequestRepository _aiRequestRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAggregateRepository<QuizGenerationAggregate> _aggregateRepository;
        private readonly IQuizProjectorService _projectorService;
        private readonly ILogger<QuizGenerationEventService> _logger;

        public QuizGenerationEventService(
            IAiService aiService,
            IQuizRepository quizRepository,
            IAiRequestRepository aiRequestRepository,
            ICurrentUserService currentUserService,
            IAggregateRepository<QuizGenerationAggregate> aggregateRepository,
            IQuizProjectorService projectorService,
            ILogger<QuizGenerationEventService> logger)
        {
            _aiService = aiService;
            _quizRepository = quizRepository;
            _aiRequestRepository = aiRequestRepository;
            _currentUserService = currentUserService;
            _aggregateRepository = aggregateRepository;
            _projectorService = projectorService;
            _logger = logger;
        }

        public async Task<QuizGenerationAggregate> GenerateQuizWithEventSourcingAsync(
            AiQuizRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var aggregateId = $"quiz-gen-{Guid.NewGuid()}";
            var aggregate = new QuizGenerationAggregate(aggregateId);
            var currentUserId = _currentUserService.GetUserId();

            try
            {
                _logger.LogInformation("Starting quiz generation with event sourcing. AggregateId: {AggregateId}", aggregateId);

                var userId = currentUserId ?? request.TeacherId ?? 0;
                if (userId == 0)
                {
                    throw new UnauthorizedAccessException("User must be authenticated to generate quiz");
                }

                var levelId = MapGradeLevelToLevelId(request.GradeLevel);

                // Event 1: Initiate generation
                aggregate.InitiateGeneration(
                    userId,
                    levelId,
                    request.Title,
                    request.GradeLevel,
                    request.Topic,
                    request.QuestionCount,
                    request.Duration,
                    JsonSerializer.Serialize(request),
                    currentUserId);

                await SaveAndProjectAsync(aggregate);
                _logger.LogInformation("Quiz generation initiated. AggregateId: {AggregateId}", aggregateId);

                // Event 2: Create AI Request
                var aiRequest = new AiRequest
                {
                    UserId = userId,
                    LevelId = levelId,
                    RequestType = RequestType.GenerateQuiz,
                    Prompt = JsonSerializer.Serialize(request),
                    Status = AiRequestStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                await _aiRequestRepository.AddAsync(aiRequest);

                aggregate.CreateAiRequest(aiRequest.AIRequestId, currentUserId);
                await SaveAndProjectAsync(aggregate);
                _logger.LogInformation("AI Request created. RequestId: {RequestId}", aiRequest.AIRequestId);

                // Event 3: Generate content with AI
                var aiResponse = await _aiService.GenerateQuizAsync(request, cancellationToken);

                aggregate.RecordAiContentGenerated(
                    JsonSerializer.Serialize(aiResponse),
                    aiResponse.Questions.Count,
                    aiResponse.TotalPoints,
                    currentUserId);

                aiRequest.Response = JsonSerializer.Serialize(aiResponse);
                aiRequest.Status = AiRequestStatus.Success;
                await _aiRequestRepository.UpdateAsync(aiRequest);

                await SaveAndProjectAsync(aggregate);
                _logger.LogInformation("AI content generated successfully. QuestionCount: {Count}", aiResponse.Questions.Count);

                // Event 4: Create Quiz entity
                var quiz = new Quiz
                {
                    TeacherId = userId,
                    LevelId = levelId,
                    Title = aiResponse.Title,
                    TimeLimit = aiResponse.Duration,
                    AttemptLimit = 3,
                    TotalScore = aiResponse.TotalPoints,
                    IsAIGenerated = true,
                    Status = QuizStatus.Draft,
                    CreatedAt = DateTime.UtcNow,
                    Questions = new List<Question>()
                };

                var questionIds = new List<int>();
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
                        Status = QuestionStatus.Approved,
                        CreatedAt = DateTime.UtcNow,
                        Answers = new List<Answer>()
                    };

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
                questionIds = quiz.Questions.Select(q => q.QuestionId).ToList();

                aggregate.CreateQuiz(
                    quiz.QuizId,
                    quiz.TeacherId,
                    quiz.LevelId,
                    quiz.Title,
                    quiz.TimeLimit,
                    quiz.TotalScore,
                    currentUserId);

                await SaveAndProjectAsync(aggregate);
                _logger.LogInformation("Quiz created in database. QuizId: {QuizId}", quiz.QuizId);

                // Event 5: Add questions
                aggregate.AddQuestions(questionIds, currentUserId);
                await SaveAndProjectAsync(aggregate);
                _logger.LogInformation("Questions added to quiz. Count: {Count}", questionIds.Count);

                // Event 6: Complete generation
                aggregate.CompleteGeneration(currentUserId);
                await SaveAndProjectAsync(aggregate);
                _logger.LogInformation("Quiz generation completed successfully. AggregateId: {AggregateId}, QuizId: {QuizId}", 
                    aggregateId, quiz.QuizId);

                return aggregate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating quiz with event sourcing. AggregateId: {AggregateId}", aggregateId);

                aggregate.FailGeneration(
                    ex.Message,
                    ex.GetType().Name,
                    ex.StackTrace ?? string.Empty,
                    currentUserId);

                await SaveAndProjectAsync(aggregate);

                throw new Exception($"Failed to generate quiz: {ex.Message}", ex);
            }
        }

        public async Task<QuizGenerationAggregate> GetGenerationHistoryAsync(string aggregateId)
        {
            return await _aggregateRepository.LoadAsync(aggregateId);
        }

        public async Task<bool> GenerationExistsAsync(string aggregateId)
        {
            return await _aggregateRepository.ExistsAsync(aggregateId);
        }

        #region Helper Methods

        private async Task SaveAndProjectAsync(QuizGenerationAggregate aggregate)
        {
            // Get uncommitted events before save
            var uncommittedEvents = aggregate.GetUncommittedEvents().ToList();
            
            // Save to event store
            await _aggregateRepository.SaveAsync(aggregate);
            
            // Project events to read models
            foreach (var @event in uncommittedEvents)
            {
                await _projectorService.ProjectEventAsync(@event);
            }
        }

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

            return Topic.Algebra;
        }

        private int MapGradeLevelToLevelId(int grade)
        {
            if (grade >= 1 && grade <= 5) return 1;
            if (grade >= 6 && grade <= 9) return 2;
            if (grade >= 10 && grade <= 12) return 3;
            return 2;
        }

        #endregion
    }
}
