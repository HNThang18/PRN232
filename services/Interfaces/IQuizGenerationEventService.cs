using applications.DTOs.Request.AI;
using repositories.EventSourcing.Aggregates;

namespace services.Interfaces
{
    public interface IQuizGenerationEventService
    {
        Task<QuizGenerationAggregate> GenerateQuizWithEventSourcingAsync(
            AiQuizRequestDto request, 
            CancellationToken cancellationToken = default);
        Task<QuizGenerationAggregate> GetGenerationHistoryAsync(string aggregateId);
        Task<bool> GenerationExistsAsync(string aggregateId);
    }
}
