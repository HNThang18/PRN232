using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;
        public AnswerService(IAnswerRepository answerRepository)
        {
            _answerRepository = answerRepository;
        }

        public async Task<Answer> CreateAsync(Answer answer)
        {
            await _answerRepository.CreateAsync(answer);
            return answer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _answerRepository.GetByIdAsync(id);
            if (existing == null) return false;
            return await _answerRepository.RemoveAsync(existing);
        }

        public async Task<List<Answer>> GetAllAsync()
        {
            return await _answerRepository.GetAllAsync();
        }

        public async Task<Answer?> GetByIdAsync(int id)
        {
            return await _answerRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(int id, Answer answer)
        {
            var existing = await _answerRepository.GetByIdAsync(id);
            if (existing == null) return false;

            // map fields
            existing.AnswerText = answer.AnswerText;
            existing.IsCorrect = answer.IsCorrect;
            existing.QuestionId = answer.QuestionId;

            await _answerRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<List<Answer>> GetByQuestionIdAsync(int questionId)
        {
            return await _answerRepository.GetByQuestionIdAsync(questionId);
        }
    }
}