using Microsoft.Extensions.Logging;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;

namespace services.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ILogger<QuizService> _logger;

        public QuizService(IQuizRepository quizRepository, ILogger<QuizService> logger)
        {
            _quizRepository = quizRepository;
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

        public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
        {
            try
            {
                return await _quizRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all quizzes");
                throw;
            }
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByTeacherIdAsync(int teacherId)
        {
            try
            {
                return await _quizRepository.GetByTeacherIdAsync(teacherId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quizzes by teacher ID: {TeacherId}", teacherId);
                throw;
            }
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByLevelIdAsync(int levelId)
        {
            try
            {
                return await _quizRepository.GetByLevelIdAsync(levelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quizzes by level ID: {LevelId}", levelId);
                throw;
            }
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByStatusAsync(QuizStatus status)
        {
            try
            {
                return await _quizRepository.GetByStatusAsync(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quizzes by status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<Quiz>> GetAiGeneratedQuizzesAsync()
        {
            try
            {
                return await _quizRepository.GetAiGeneratedQuizzesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AI generated quizzes");
                throw;
            }
        }

        public async Task<Quiz> CreateQuizAsync(Quiz quiz)
        {
            try
            {
                if (quiz == null)
                    throw new ArgumentNullException(nameof(quiz));

                quiz.CreatedAt = DateTime.UtcNow;
                quiz.Status = QuizStatus.Draft; // Default status

                return await _quizRepository.AddAsync(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz: {Title}", quiz?.Title);
                throw;
            }
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {
            try
            {
                if (quiz == null)
                    throw new ArgumentNullException(nameof(quiz));

                var existingQuiz = await _quizRepository.GetByIdAsync(quiz.QuizId);
                if (existingQuiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quiz.QuizId} not found");

                await _quizRepository.UpdateAsync(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quiz: {QuizId}", quiz?.QuizId);
                throw;
            }
        }

        public async Task DeleteQuizAsync(int quizId)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");

                await _quizRepository.DeleteAsync(quizId);
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

        public async Task PublishQuizAsync(int quizId)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");

                if (quiz.Questions == null || !quiz.Questions.Any())
                    throw new InvalidOperationException("Cannot publish quiz without questions");

                quiz.Status = QuizStatus.Published;
                await _quizRepository.UpdateAsync(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing quiz: {QuizId}", quizId);
                throw;
            }
        }

        public async Task UnpublishQuizAsync(int quizId)
        {
            try
            {
                var quiz = await _quizRepository.GetByIdAsync(quizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with ID {quizId} not found");

                quiz.Status = QuizStatus.Draft;
                await _quizRepository.UpdateAsync(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing quiz: {QuizId}", quizId);
                throw;
            }
        }
    }
}
