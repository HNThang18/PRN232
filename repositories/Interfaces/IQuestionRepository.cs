using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace repositories.Interfaces
{
    public interface IQuestionRepository
    {
        // Task AddQuestionAsync(Question question);
        // Task<List<Question>> GetQuestionsWithCorrectAnswersAsync(List<int> questionIds);
        // Task<Question?> GetByIdAsync(int questionId);
        // Task<IEnumerable<Question>> GetByQuizIdAsync(int quizId);
        // Task UpdateAsync(Question question);
        Task<IEnumerable<Question>> GetAvailableQuestionsAsync(int? levelId = null, int? difficultyId = null, string? topic = null, string? searchTerm = null);

        // CRUD
        Task<List<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task<int> CreateAsync(Question question);
        Task<int> UpdateAsync(Question question);
        Task<bool> RemoveAsync(Question question);

        Task<List<Question>> GetByQuizIdAsync(int quizId);
    }
}