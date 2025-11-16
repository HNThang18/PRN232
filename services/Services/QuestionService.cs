using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public async Task AddQuestionAsync(Question question)
        {
            await _questionRepository.AddQuestionAsync(question);
        }

        public async Task<IEnumerable<Question>> GetAvailableQuestionsAsync(
            int? levelId = null, 
            int? difficultyId = null, 
            string? topic = null, 
            string? searchTerm = null)
        {
            return await _questionRepository.GetAvailableQuestionsAsync(levelId, difficultyId, topic, searchTerm);
        }
    }
}
