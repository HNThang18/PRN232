using applications.DTOs.Quiz;
using repositories.Models;

namespace services.Interfaces
{
    public interface IQuizService
    {
        Task<Quiz?> GetQuizByIdAsync(int quizId);
        Task<QuizDetailResponseDto?> GetQuizDetailByIdAsync(int quizId);
        Task<IEnumerable<QuizResponseDto>> GetQuizzesByTeacherIdAsync(int teacherId, int page, int limit);
        Task<int> GetQuizCountByTeacherIdAsync(int teacherId, QuizStatus? status);
        Task<IEnumerable<QuizResponseDto>> SearchQuizzesAsync(string? keyword, int? levelId, QuizStatus? status, int? teacherId, int page, int limit);
        Task<int> GetSearchCountAsync(string? keyword, int? levelId, QuizStatus? status, int? teacherId);
        Task<Quiz> CreateQuizAsync(int teacherId, CreateQuizRequestDto request);
        Task<Quiz> UpdateQuizAsync(int quizId, int teacherId, UpdateQuizRequestDto request);
        Task DeleteQuizAsync(int quizId, int teacherId);
        Task<bool> QuizExistsAsync(int quizId);
        Task PublishQuizAsync(int quizId, int teacherId);
        Task UnpublishQuizAsync(int quizId, int teacherId);
        Task<QuizStatisticsDto> GetQuizStatisticsAsync(int quizId);
        Task AddQuestionsToQuizAsync(int quizId, int teacherId, List<int> questionIds);
        Task RemoveQuestionFromQuizAsync(int quizId, int teacherId, int questionId);
    }
}
