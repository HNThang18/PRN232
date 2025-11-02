using repositories.Models;

namespace repositories.Interfaces
{
    public interface IQuizRepository
    {
        Task<Quiz?> GetByIdAsync(int quizId);
        Task<IEnumerable<Quiz>> GetAllAsync();
        Task<IEnumerable<Quiz>> GetByTeacherIdAsync(int teacherId);
        Task<IEnumerable<Quiz>> GetByLevelIdAsync(int levelId);
        Task<IEnumerable<Quiz>> GetByStatusAsync(QuizStatus status);
        Task<IEnumerable<Quiz>> GetAiGeneratedQuizzesAsync();
        Task<Quiz> AddAsync(Quiz quiz);
        Task UpdateAsync(Quiz quiz);
        Task DeleteAsync(int quizId);
        Task<bool> ExistsAsync(int quizId);

        Task<Quiz> GetQuizWithDetailsAsync(int quizId);
    }
}