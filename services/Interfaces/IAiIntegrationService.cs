using applications.DTOs.Request.AI;
using applications.DTOs.Response.AI;
using repositories.Models;

namespace services.Interfaces
{
    public interface IAiIntegrationService
    {
        Task<LessonPlan> GenerateAndSaveLessonPlanAsync(
            AiLessonPlanRequestDto request, 
            CancellationToken cancellationToken = default);
        Task<List<Question>> GenerateAndSaveQuestionsAsync(
            AiQuestionRequestDto request, 
            CancellationToken cancellationToken = default);
        Task<Quiz> GenerateAndSaveQuizAsync(
            AiQuizRequestDto request, 
            CancellationToken cancellationToken = default);
    }
}
