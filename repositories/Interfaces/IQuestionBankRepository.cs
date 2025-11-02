using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace repositories.Interfaces
{
    public interface IQuestionBankRepository
    {
        Task<IEnumerable<QuestionBank>> GetAllQuestionBanksAsync();
        Task<QuestionBank?> GetQuestionBankByIdAsync(int id);
        Task AddQuestionBankAsync(QuestionBank questionBank);
        Task UpdateQuestionBankAsync(QuestionBank questionBank);
        Task DeleteQuestionBankAsync(int id);
    }
}
