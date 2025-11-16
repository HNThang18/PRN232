using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.Interfaces
{
    public interface IAnswerService
    {
        Task<List<Answer>> GetAllAsync();
        Task<Answer?> GetByIdAsync(int id);
        Task<Answer> CreateAsync(Answer answer);
        Task<bool> UpdateAsync(int id, Answer answer);
        Task<bool> DeleteAsync(int id);
        Task<List<Answer>> GetByQuestionIdAsync(int questionId);
    }
}