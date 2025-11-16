using applications.DTOs.Question;
using applications.DTOs.Quiz;
using Microsoft.Extensions.Logging;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;

namespace services.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ILogger<QuizService> _logger;

        public QuizService(
            IQuizRepository quizRepository,
            IQuestionRepository questionRepository,
            ISubmissionRepository submissionRepository,
            ILogger<QuizService> logger)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _submissionRepository = submissionRepository;
            _logger = logger;
        }

        public async Task<Quiz?> GetQuizByIdAsync(int quizId)
        {
            try
            {
                return await _quizRepository.GetByIdAsync(quizId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz by ID: {QuizId}", quizId);
                throw;
            }
        }

        public async Task<QuizDetailResponseDto?> GetQuizDetailByIdAsync(int quizId)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null) return null;

                return new QuizDetailResponseDto
                {
                    QuizId = quiz.QuizId,
                    Title = quiz.Title,
                    LevelId = quiz.LevelId,
                    LevelName = quiz.Level?.LevelName ?? "",
                    TeacherId = quiz.TeacherId,
                    TeacherName = quiz.Teacher?.Username ?? "",
                    TimeLimit = quiz.TimeLimit,
                    AttemptLimit = quiz.AttemptLimit,
                    TotalScore = quiz.TotalScore,
                    IsAIGenerated = quiz.IsAIGenerated,
                    Status = quiz.Status.ToString(),
                    CreatedAt = quiz.CreatedAt,
                    PublishedAt = quiz.PublishedAt,
                    Questions = quiz.Questions?.Select(q => new QuestionResponseDto
                    {
                        QuestionId = q.QuestionId,
                        Topic = q.Topic.ToString(),
                        QuestionText = q.QuestionText,
                        QuestionType = q.QuestionType.ToString(),
                        CorrectAnswer = q.CorrectAnswer,
                        Explanation = q.Explanation,
                        Tags = q.Tags,
                        IsAIGenerated = q.IsAIGenerated,
                        Status = q.Status.ToString(),
                        DifficultyId = q.DifficultyId,
                        DifficultyName = q.Difficulty?.Name,
                        Answers = q.Answers?.Select(a => new AnswerResponseDto
                        {
                            AnswerId = a.AnswerId,
                            AnswerText = a.AnswerText,
                            IsCorrect = a.IsCorrect
                        }).ToList() ?? new List<AnswerResponseDto>()
                    }).ToList() ?? new List<QuestionResponseDto>(),
                    SubmissionCount = quiz.Submissions?.Count ?? 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz detail by ID: {QuizId}", quizId);
                throw;
            }
        }

        public async Task<IEnumerable<QuizResponseDto>> GetQuizzesByTeacherIdAsync(int teacherId, int page, int limit)
        {
            try
            {
                var quizzes = await _quizRepository.GetByTeacherIdAsync(teacherId, page, limit);
                return quizzes.Select(q => new QuizResponseDto
                {
                    QuizId = q.QuizId,
                    Title = q.Title,
                    LevelId = q.LevelId,
                    LevelName = q.Level?.LevelName ?? "",
                    TeacherId = q.TeacherId,
                    TeacherName = q.Teacher?.Username ?? "",
                    TimeLimit = q.TimeLimit,
                    AttemptLimit = q.AttemptLimit,
                    TotalScore = q.TotalScore,
                    IsAIGenerated = q.IsAIGenerated,
                    Status = q.Status.ToString(),
                    CreatedAt = q.CreatedAt,
                    PublishedAt = q.PublishedAt,
                    QuestionCount = q.Questions?.Count ?? 0,
                    SubmissionCount = q.Submissions?.Count ?? 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quizzes by teacher ID: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<int> GetQuizCountByTeacherIdAsync(int teacherId, QuizStatus? status)
        {
            try
            {
                return await _quizRepository.GetCountByTeacherIdAsync(teacherId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz count by teacher ID: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<IEnumerable<QuizResponseDto>> SearchQuizzesAsync(string? keyword, int? levelId, QuizStatus? status, int? teacherId, int page, int limit)
        {
            try
            {
                var quizzes = await _quizRepository.SearchQuizzesAsync(keyword, levelId, status, teacherId, page, limit);
                return quizzes.Select(q => new QuizResponseDto
                {
                    QuizId = q.QuizId,
                    Title = q.Title,
                    LevelId = q.LevelId,
                    LevelName = q.Level?.LevelName ?? "",
                    TeacherId = q.TeacherId,
                    TeacherName = q.Teacher?.Username ?? "",
                    TimeLimit = q.TimeLimit,
                    AttemptLimit = q.AttemptLimit,
                    TotalScore = q.TotalScore,
                    IsAIGenerated = q.IsAIGenerated,
                    Status = q.Status.ToString(),
                    CreatedAt = q.CreatedAt,
                    PublishedAt = q.PublishedAt,
                    QuestionCount = q.Questions?.Count ?? 0,
                    SubmissionCount = q.Submissions?.Count ?? 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching quizzes");
                throw;
            }
        }

        public async Task<int> GetSearchCountAsync(string? keyword, int? levelId, QuizStatus? status, int? teacherId)
        {
            try
            {
                return await _quizRepository.GetSearchCountAsync(keyword, levelId, status, teacherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting search count");
                throw;
            }
        }

        public async Task<Quiz> CreateQuizAsync(int teacherId, CreateQuizRequestDto request)
        {
            try
            {
                var quiz = new Quiz
                {
                    TeacherId = teacherId,
                    LevelId = request.LevelId,
                    Title = request.Title,
                    TimeLimit = request.TimeLimit,
                    AttemptLimit = request.AttemptLimit,
                    Status = QuizStatus.Draft,
                    IsAIGenerated = false,
                    CreatedAt = DateTime.UtcNow,
                    TotalScore = 0
                };

                var createdQuiz = await _quizRepository.AddAsync(quiz);

                if (request.QuestionIds != null && request.QuestionIds.Any())
                {
                    var questions = new List<Question>();
                    int totalScore = 0;

                    foreach (var questionId in request.QuestionIds)
                    {
                        var question = await _questionRepository.GetByIdAsync(questionId);
                        if (question != null)
                        {
                            question.QuizId = createdQuiz.QuizId;
                            await _questionRepository.UpdateAsync(question);
                            questions.Add(question);
                            totalScore += 10;
                        }
                    }

                    createdQuiz.TotalScore = totalScore;
                    await _quizRepository.UpdateAsync(createdQuiz);
                }

                return createdQuiz;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz: {Title}", request.Title);
                throw;
            }
        }

        public async Task<Quiz> UpdateQuizAsync(int quizId, int teacherId, UpdateQuizRequestDto request)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");

                if (quiz.TeacherId != teacherId)
                    throw new UnauthorizedAccessException("You are not authorized to update this quiz");

                if (quiz.Status == QuizStatus.Published)
                    throw new InvalidOperationException("Cannot update a published quiz");

                if (!string.IsNullOrWhiteSpace(request.Title))
                    quiz.Title = request.Title;

                if (request.TimeLimit.HasValue)
                    quiz.TimeLimit = request.TimeLimit.Value;

                if (request.AttemptLimit.HasValue)
                    quiz.AttemptLimit = request.AttemptLimit.Value;

                if (request.QuestionIds != null && request.QuestionIds.Any())
                {
                    var existingQuestions = await _questionRepository.GetByQuizIdAsync(quizId);
                    foreach (var q in existingQuestions)
                    {
                        q.QuizId = null;
                        await _questionRepository.UpdateAsync(q);
                    }

                    int totalScore = 0;
                    foreach (var questionId in request.QuestionIds)
                    {
                        var question = await _questionRepository.GetByIdAsync(questionId);
                        if (question != null)
                        {
                            question.QuizId = quizId;
                            await _questionRepository.UpdateAsync(question);
                            totalScore += 10;
                        }
                    }
                    quiz.TotalScore = totalScore;
                }

                await _quizRepository.UpdateAsync(quiz);
                return quiz;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quiz: {QuizId}", quizId);
                throw;
            }
        }

        public async Task DeleteQuizAsync(int quizId, int teacherId)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");

                if (quiz.TeacherId != teacherId)
                    throw new UnauthorizedAccessException("You are not authorized to delete this quiz");

                var submissionCount = await _submissionRepository.GetSubmissionCountByQuizIdAsync(quizId);
                if (submissionCount > 0)
                {
                    quiz.Status = QuizStatus.Deleted;
                    await _quizRepository.UpdateAsync(quiz);
                }
                else
                {
                    await _quizRepository.DeleteAsync(quizId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quiz: {QuizId}", quizId);
                throw;
            }
        }

        public async Task<bool> QuizExistsAsync(int quizId)
        {
            try
            {
                return await _quizRepository.ExistsAsync(quizId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if quiz exists: {QuizId}", quizId);
                throw;
            }
        }

        public async Task PublishQuizAsync(int quizId, int teacherId)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");

                if (quiz.TeacherId != teacherId)
                    throw new UnauthorizedAccessException("You are not authorized to publish this quiz");

                if (quiz.Questions == null || !quiz.Questions.Any())
                    throw new InvalidOperationException("Cannot publish quiz without questions");

                quiz.Status = QuizStatus.Published;
                quiz.PublishedAt = DateTime.UtcNow;
                await _quizRepository.UpdateAsync(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing quiz: {QuizId}", quizId);
                throw;
            }
        }

        public async Task UnpublishQuizAsync(int quizId, int teacherId)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");

                if (quiz.TeacherId != teacherId)
                    throw new UnauthorizedAccessException("You are not authorized to unpublish this quiz");

                quiz.Status = QuizStatus.Draft;
                quiz.PublishedAt = null;
                await _quizRepository.UpdateAsync(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing quiz: {QuizId}", quizId);
                throw;
            }
        }

        public async Task<QuizStatisticsDto> GetQuizStatisticsAsync(int quizId)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");

                var submissions = await _submissionRepository.GetSubmissionsByQuizIdAsync(quizId);
                var completedSubmissions = submissions.Where(s => s.Status == SubissionStatus.Completed).ToList();

                return new QuizStatisticsDto
                {
                    QuizId = quiz.QuizId,
                    Title = quiz.Title,
                    TotalSubmissions = submissions.Count(),
                    CompletedSubmissions = completedSubmissions.Count,
                    InProgressSubmissions = submissions.Count(s => s.Status == SubissionStatus.InProgress),
                    AverageScore = completedSubmissions.Any() ? completedSubmissions.Average(s => s.Score) : 0,
                    HighestScore = completedSubmissions.Any() ? completedSubmissions.Max(s => s.Score) : 0,
                    LowestScore = completedSubmissions.Any() ? completedSubmissions.Min(s => s.Score) : 0,
                    AverageDuration = completedSubmissions.Any() ? completedSubmissions.Average(s => s.DurationTaken) : 0,
                    TotalStudents = submissions.Select(s => s.StudentId).Distinct().Count()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz statistics: {QuizId}", quizId);
                throw;
            }
        }

        public async Task AddQuestionsToQuizAsync(int quizId, int teacherId, List<int> questionIds)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");

                if (quiz.TeacherId != teacherId)
                    throw new UnauthorizedAccessException("You are not authorized to modify this quiz");

                if (quiz.Status != QuizStatus.Draft)
                    throw new InvalidOperationException("Cannot add questions to a published or deleted quiz");

                foreach (var questionId in questionIds)
                {
                    var question = await _questionRepository.GetByIdAsync(questionId);
                    if (question == null)
                    {
                        _logger.LogWarning("Question with ID {QuestionId} not found, skipping", questionId);
                        continue;
                    }

                    if (question.Status != QuestionStatus.Approved)
                    {
                        _logger.LogWarning("Question with ID {QuestionId} is not approved, skipping", questionId);
                        continue;
                    }

                    if (question.QuizId.HasValue)
                    {
                        _logger.LogWarning("Question with ID {QuestionId} is already assigned to a quiz, skipping", questionId);
                        continue;
                    }

                    question.QuizId = quizId;
                    await _questionRepository.UpdateAsync(question);
                }

                var updatedQuiz = await _quizRepository.GetByIdAsync(quizId);
                if (updatedQuiz?.Questions != null)
                {
                    updatedQuiz.TotalScore = updatedQuiz.Questions.Count * 100;
                    await _quizRepository.UpdateAsync(updatedQuiz);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding questions to quiz: {QuizId}", quizId);
                throw;
            }
        }

        public async Task RemoveQuestionFromQuizAsync(int quizId, int teacherId, int questionId)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");

                if (quiz.TeacherId != teacherId)
                    throw new UnauthorizedAccessException("You are not authorized to modify this quiz");

                if (quiz.Status != QuizStatus.Draft)
                    throw new InvalidOperationException("Cannot remove questions from a published or deleted quiz");

                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question == null)
                    throw new KeyNotFoundException($"Question with ID {questionId} not found");

                if (question.QuizId != quizId)
                    throw new InvalidOperationException("Question does not belong to this quiz");

                question.QuizId = null;
                await _questionRepository.UpdateAsync(question);

                var updatedQuiz = await _quizRepository.GetByIdAsync(quizId);
                if (updatedQuiz?.Questions != null)
                {
                    updatedQuiz.TotalScore = updatedQuiz.Questions.Count * 100;
                    await _quizRepository.UpdateAsync(updatedQuiz);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing question from quiz: {QuizId}, {QuestionId}", quizId, questionId);
                throw;
            }
        }
    }
}
