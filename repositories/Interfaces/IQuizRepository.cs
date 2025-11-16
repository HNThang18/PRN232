using repositories.Models;

namespace repositories.Interfaces
{
    public interface IQuizRepository
    {
        Task<Quiz?> GetByIdAsync(int quizId);
        Task<IEnumerable<Quiz>> GetAllAsync();
        Task<IEnumerable<Quiz>> GetByTeacherIdAsync(int teacherId, int page, int limit);
        Task<int> GetCountByTeacherIdAsync(int teacherId, QuizStatus? status);
        Task<IEnumerable<Quiz>> GetByLevelIdAsync(int levelId);
        Task<IEnumerable<Quiz>> GetByStatusAsync(QuizStatus status, int page, int limit);
        Task<int> GetCountByStatusAsync(QuizStatus? status);
        Task<IEnumerable<Quiz>> GetAiGeneratedQuizzesAsync();
        Task<Quiz> AddAsync(Quiz quiz);
        Task UpdateAsync(Quiz quiz);
        Task DeleteAsync(int quizId);
        Task<bool> ExistsAsync(int quizId);
        Task<Quiz> GetQuizWithDetailsAsync(int quizId);
        Task<IEnumerable<Quiz>> SearchQuizzesAsync(string? keyword, int? levelId, QuizStatus? status, int? teacherId, int page, int limit);
        Task<int> GetSearchCountAsync(string? keyword, int? levelId, QuizStatus? status, int? teacherId);
    }
}