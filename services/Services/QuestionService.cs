//using repositories.Interfaces;
//using repositories.Models;
//using services.Interfaces;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace services.Services
//{
//    public class QuestionService : IQuestionService
//    {
//        private readonly IQuestionRepository _questionRepository;
//        public QuestionService(IQuestionRepository questionRepository)
//        {
//            _questionRepository = questionRepository;
//        }

//        // public async Task AddQuestionAsync(Question question)
//        // {
//        //     await _questionRepository.AddQuestionAsync(question);
//        // }
//        public async Task<IEnumerable<Question>> GetAvailableQuestionsAsync(
//            int? levelId = null,
//            int? difficultyId = null,
//            string? topic = null,
//            string? searchTerm = null)
//        {
//            return await _questionRepository.GetAvailableQuestionsAsync(levelId, difficultyId, topic, searchTerm);
//        }
//        public async Task<List<Question>> GetAllAsync()
//        {
//            return await _questionRepository.GetAllAsync();
//        }

//        public async Task<Question?> GetByIdAsync(int id)
//        {
//            return await _questionRepository.GetByIdAsync(id);
//        }

//        public async Task<Question> CreateAsync(Question question)
//        {
//            await _questionRepository.CreateAsync(question);
//            return question;
//        }

//        public async Task<bool> UpdateAsync(int id, Question question)
//        {
//            var existing = await _questionRepository.GetByIdAsync(id);
//            if (existing == null) return false;

//            // map fields
//            existing.QuizId = question.QuizId;
//            existing.QuestionBankId = question.QuestionBankId;
//            existing.DifficultyId = question.DifficultyId;
//            existing.Topic = question.Topic;
//            existing.QuestionText = question.QuestionText;
//            existing.QuestionType = question.QuestionType;
//            existing.CorrectAnswer = question.CorrectAnswer;
//            existing.Explanation = question.Explanation;
//            existing.Tags = question.Tags;
//            existing.IsAIGenerated = question.IsAIGenerated;
//            existing.Status = question.Status;

//            await _questionRepository.UpdateAsync(existing);
//            return true;
//        }

//        public async Task<bool> DeleteAsync(int id)
//        {
//            var existing = await _questionRepository.GetByIdAsync(id);
//            if (existing == null) return false;
//            return await _questionRepository.RemoveAsync(existing);
//        }

//        public async Task<List<Question>> GetByQuizIdAsync(int quizId)
//        {
//            return await _questionRepository.GetByQuizIdAsync(quizId);
//        }
//    }
//}
