using repositories.EventSourcing.Events;

namespace repositories.EventSourcing.Repositories
{
    public interface IEventStoreRepository
    {
        Task SaveEventsAsync(string aggregateId, IEnumerable<IEvent> events, int expectedVersion);
        Task<IEnumerable<IEvent>> GetEventsAsync(string aggregateId);
        Task<IEnumerable<IEvent>> GetEventsAsync(string aggregateId, int fromVersion);
        Task<bool> AggregateExistsAsync(string aggregateId);
        Task<int> GetLatestVersionAsync(string aggregateId);
    }
}
