using applications.DTOs.Request.AI;
using applications.DTOs.Response.AI;

namespace services.Interfaces
{
    public interface IAiService
    {
        Task<AiLessonPlanResponseDto> GenerateLessonPlanAsync(AiLessonPlanRequestDto request, CancellationToken cancellationToken = default);
        Task<AiQuestionResponseDto> GenerateQuestionsAsync(AiQuestionRequestDto request, CancellationToken cancellationToken = default);
        Task<AiQuizResponseDto> GenerateQuizAsync(AiQuizRequestDto request, CancellationToken cancellationToken = default);
        Task<AiChatResponseDto> ChatAsync(AiChatRequestDto request, CancellationToken cancellationToken = default);
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    }
}
