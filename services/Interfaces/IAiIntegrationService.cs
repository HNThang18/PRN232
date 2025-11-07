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
        Task<IEnumerable<AiRequestHistoryDto>> GetRequestHistoryAsync(
            int userId,
            RequestType? requestType = null,
            AiRequestStatus? status = null,
            string? search = null,
            int page = 1,
            int limit = 20,
            CancellationToken cancellationToken = default);
        Task<int> GetRequestCountAsync(
            int userId,
            RequestType? requestType = null,
            AiRequestStatus? status = null,
            string? search = null,
            CancellationToken cancellationToken = default);
        Task<AiRequestDetailDto?> GetRequestDetailsAsync(int requestId, CancellationToken cancellationToken = default);
    }
}
