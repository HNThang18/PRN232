using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task AddQuestionAsync(Question question);
        Task<List<Question>> GetQuestionsWithCorrectAnswersAsync(List<int> questionIds);

        // CRUD
        Task<List<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task<int> CreateAsync(Question question);
        Task<int> UpdateAsync(Question question);
        Task<bool> RemoveAsync(Question question);

        Task<List<Question>> GetByQuizIdAsync(int quizId);
    }
}