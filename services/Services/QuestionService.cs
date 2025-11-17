using applications.DTOs.Question;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace services.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;

        public QuestionService(
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
        }

        public async Task<IEnumerable<QuestionResponseDto>> GetAvailableQuestionsAsync(
            int? levelId = null,
            int? difficultyId = null,
            string? topic = null,
            string? searchTerm = null)
        {
            var questions = await _questionRepository.GetAvailableQuestionsAsync(
                levelId, difficultyId, topic, searchTerm);

            return questions.Select(MapToResponseDto).ToList();
        }

        public async Task<IEnumerable<QuestionResponseDto>> GetAllAsync()
        {
            var questions = await _questionRepository.GetAllAsync();
            return questions.Select(MapToResponseDto).ToList();
        }

        public async Task<QuestionResponseDto?> GetByIdAsync(int id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            return question != null ? MapToResponseDto(question) : null;
        }

        public async Task<QuestionResponseDto> CreateAsync(CreateQuestionRequestDto request)
        {
            if (!Enum.TryParse<Topic>(request.Topic, true, out var topicEnum))
            {
                throw new ArgumentException($"Invalid topic: {request.Topic}");
            }

            if (!Enum.TryParse<QuestionType>(request.QuestionType, true, out var questionTypeEnum))
            {
                throw new ArgumentException($"Invalid question type: {request.QuestionType}");
            }

            var question = new Question
            {
                QuestionBankId = request.QuestionBankId,
                DifficultyId = request.DifficultyId,
                Topic = topicEnum,
                QuestionText = request.QuestionText,
                QuestionType = questionTypeEnum,
                CorrectAnswer = request.CorrectAnswer,
                Explanation = request.Explanation,
                Tags = request.Tags,
                IsAIGenerated = request.IsAIGenerated,
                Status = QuestionStatus.PendingReview,
                CreatedAt = DateTime.UtcNow
            };

            await _questionRepository.CreateAsync(question);

            if (request.Answers != null && request.Answers.Count > 0)
            {
                foreach (var answerDto in request.Answers)
                {
                    var answer = new Answer
                    {
                        QuestionId = question.QuestionId,
                        AnswerText = answerDto.AnswerText,
                        IsCorrect = answerDto.IsCorrect
                    };
                    await _answerRepository.CreateAsync(answer);
                }
            }

            return MapToResponseDto(question);
        }

        public async Task<bool> UpdateAsync(int id, UpdateQuestionRequestDto request)
        {
            var existing = await _questionRepository.GetByIdAsync(id);
            if (existing == null) return false;

            if (request.QuestionBankId.HasValue)
                existing.QuestionBankId = request.QuestionBankId.Value;

            if (request.DifficultyId.HasValue)
                existing.DifficultyId = request.DifficultyId.Value;

            if (!string.IsNullOrWhiteSpace(request.Topic))
            {
                if (System.Enum.TryParse<Topic>(request.Topic, true, out var topicEnum))
                    existing.Topic = topicEnum;
            }

            existing.QuestionText = request.QuestionText;

            if (!string.IsNullOrWhiteSpace(request.QuestionType))
            {
                if (System.Enum.TryParse<QuestionType>(request.QuestionType, true, out var questionTypeEnum))
                    existing.QuestionType = questionTypeEnum;
            }

            existing.CorrectAnswer = request.CorrectAnswer;
            existing.Explanation = request.Explanation;
            existing.Tags = request.Tags;

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (System.Enum.TryParse<QuestionStatus>(request.Status, true, out var statusEnum))
                    existing.Status = statusEnum;
            }

            await _questionRepository.UpdateAsync(existing);

            if (request.Answers != null)
            {
                var existingAnswers = await _answerRepository.GetByQuestionIdAsync(id);

                var requestAnswerIds = request.Answers
                    .Where(a => a.AnswerId.HasValue)
                    .Select(a => a.AnswerId!.Value)
                    .ToList();

                foreach (var existingAnswer in existingAnswers)
                {
                    if (!requestAnswerIds.Contains(existingAnswer.AnswerId))
                    {
                        await _answerRepository.RemoveAsync(existingAnswer);
                    }
                }

                foreach (var answerDto in request.Answers)
                {
                    if (answerDto.AnswerId.HasValue)
                    {
                        var existingAnswer = existingAnswers.FirstOrDefault(a => a.AnswerId == answerDto.AnswerId.Value);
                        if (existingAnswer != null)
                        {
                            existingAnswer.AnswerText = answerDto.AnswerText;
                            existingAnswer.IsCorrect = answerDto.IsCorrect;
                            await _answerRepository.UpdateAsync(existingAnswer);
                        }
                    }
                    else
                    {
                        var newAnswer = new Answer
                        {
                            QuestionId = id,
                            AnswerText = answerDto.AnswerText,
                            IsCorrect = answerDto.IsCorrect
                        };
                        await _answerRepository.CreateAsync(newAnswer);
                    }
                }
            }

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _questionRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Status = QuestionStatus.Deleted;
            await _questionRepository.UpdateAsync(existing);
            
            return true;
        }

        public async Task<IEnumerable<QuestionResponseDto>> GetByQuizIdAsync(int quizId)
        {
            var questions = await _questionRepository.GetByQuizIdAsync(quizId);
            return questions.Select(MapToResponseDto).ToList();
        }

        private QuestionResponseDto MapToResponseDto(Question question)
        {
            return new QuestionResponseDto
            {
                QuestionId = question.QuestionId,
                Topic = question.Topic.ToString(),
                QuestionText = question.QuestionText,
                QuestionType = question.QuestionType.ToString(),
                CorrectAnswer = question.CorrectAnswer,
                Explanation = question.Explanation,
                Tags = question.Tags,
                IsAIGenerated = question.IsAIGenerated,
                Status = question.Status.ToString(),
                DifficultyId = question.DifficultyId,
                DifficultyName = question.Difficulty?.Name,
                Answers = question.Answers?.Select(a => new AnswerResponseDto
                {
                    AnswerId = a.AnswerId,
                    AnswerText = a.AnswerText,
                    IsCorrect = a.IsCorrect
                }).ToList() ?? new List<AnswerResponseDto>()
            };
        }
    }
}
