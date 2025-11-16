using repositories.Models;

namespace services.Interfaces
{
    public interface IQuestionService
    {
        Task AddQuestionAsync(Question question);
        Task<IEnumerable<Question>> GetAvailableQuestionsAsync(int? levelId = null, int? difficultyId = null, string? topic = null, string? searchTerm = null);
    }
}