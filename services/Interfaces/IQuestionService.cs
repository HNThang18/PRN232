using applications.DTOs.Question;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.Interfaces
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionResponseDto>> GetAvailableQuestionsAsync(
            int? levelId = null, 
            int? difficultyId = null, 
            string? topic = null, 
            string? searchTerm = null);

        Task<IEnumerable<QuestionResponseDto>> GetAllAsync();
        
        Task<QuestionResponseDto?> GetByIdAsync(int id);
        
        Task<QuestionResponseDto> CreateAsync(CreateQuestionRequestDto request);
        
        Task<bool> UpdateAsync(int id, UpdateQuestionRequestDto request);
        
        Task<bool> DeleteAsync(int id);
        
        Task<IEnumerable<QuestionResponseDto>> GetByQuizIdAsync(int quizId);
    }
}