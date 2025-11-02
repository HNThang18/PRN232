using repositories.Models;
using repositories.Interfaces;
using services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.Services
{
    public class QuestionBankService : IQuestionBankService
    {
        private readonly IQuestionBankRepository _questionBankRepository;

        public QuestionBankService(IQuestionBankRepository questionBankRepository)
        {
            _questionBankRepository = questionBankRepository;
        }

        public async Task<IEnumerable<QuestionBank>> GetAllQuestionBanksAsync()
        {
            return await _questionBankRepository.GetAllQuestionBanksAsync();
        }

        public async Task<QuestionBank?> GetQuestionBankByIdAsync(int id)
        {
            return await _questionBankRepository.GetQuestionBankByIdAsync(id);
        }

        public async Task AddQuestionBankAsync(QuestionBank questionBank)
        {
            await _questionBankRepository.AddQuestionBankAsync(questionBank);
        }

        public async Task UpdateQuestionBankAsync(QuestionBank questionBank)
        {
            await _questionBankRepository.UpdateQuestionBankAsync(questionBank);
        }

        public async Task DeleteQuestionBankAsync(int id)
        {
            await _questionBankRepository.DeleteQuestionBankAsync(id);
        }
    }
}
