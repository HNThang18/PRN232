using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.Interfaces
{
    public interface IQuestionBankService
    {
        Task<IEnumerable<QuestionBank>> GetAllQuestionBanksAsync();
        Task<QuestionBank?> GetQuestionBankByIdAsync(int id);
        Task AddQuestionBankAsync(QuestionBank questionBank);
        Task UpdateQuestionBankAsync(QuestionBank questionBank);
        Task DeleteQuestionBankAsync(int id);
    }
}
