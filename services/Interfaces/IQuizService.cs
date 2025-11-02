using repositories.Models;

namespace services.Interfaces
{
    public interface IQuizService
    {
        Task<Quiz?> GetQuizByIdAsync(int quizId);
        Task<IEnumerable<Quiz>> GetAllQuizzesAsync();
        Task<IEnumerable<Quiz>> GetQuizzesByTeacherIdAsync(int teacherId);
        Task<IEnumerable<Quiz>> GetQuizzesByLevelIdAsync(int levelId);
        Task<IEnumerable<Quiz>> GetQuizzesByStatusAsync(QuizStatus status);
        Task<IEnumerable<Quiz>> GetAiGeneratedQuizzesAsync();
        Task<Quiz> CreateQuizAsync(Quiz quiz);
        Task UpdateQuizAsync(Quiz quiz);
        Task DeleteQuizAsync(int quizId);
        Task<bool> QuizExistsAsync(int quizId);
        Task PublishQuizAsync(int quizId);
        Task UnpublishQuizAsync(int quizId);
    }
}
