using repositories.EventSourcing.Aggregates;
using repositories.EventSourcing.Events;

namespace services.EventSourcing.Projectors
{
    public interface IQuizProjectorService
    {
        Task ProjectEventAsync(IEvent @event);
        Task RebuildProjectionsAsync();
    }
}
