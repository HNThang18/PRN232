using repositories.EventSourcing.Aggregates;

namespace services.Interfaces
{
    public interface IAggregateRepository<T> where T : AggregateRoot
    {
        Task<T> LoadAsync(string aggregateId);
        Task SaveAsync(T aggregate);
        Task<bool> ExistsAsync(string aggregateId);
    }
}
