using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace repositories.Interfaces
{
    public interface IAnswerRepository
    {
        Task<List<Answer>> GetAllAsync();
        Task<Answer?> GetByIdAsync(int id);
        Task<int> CreateAsync(Answer answer);
        Task<int> UpdateAsync(Answer answer);
        Task<bool> RemoveAsync(Answer answer);
        Task<List<Answer>> GetByQuestionIdAsync(int questionId);
    }
}