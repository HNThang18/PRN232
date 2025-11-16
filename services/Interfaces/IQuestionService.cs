using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.Interfaces
{
    public interface IQuestionService
    {
        // Task AddQuestionAsync(Question question);
        Task<IEnumerable<Question>> GetAvailableQuestionsAsync(int? levelId = null, int? difficultyId = null, string? topic = null, string? searchTerm = null);

        Task<List<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task<Question> CreateAsync(Question question);
        Task<bool> UpdateAsync(int id, Question question);
        Task<bool> DeleteAsync(int id);
        Task<List<Question>> GetByQuizIdAsync(int quizId);
    }
}