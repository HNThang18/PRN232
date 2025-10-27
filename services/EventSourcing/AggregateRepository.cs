using repositories.EventSourcing.Aggregates;
using repositories.EventSourcing.Repositories;
using services.Interfaces;

namespace services.EventSourcing
{
    public class AggregateRepository<T> : IAggregateRepository<T> where T : AggregateRoot
    {
        private readonly IEventStoreRepository _eventStore;

        public AggregateRepository(IEventStoreRepository eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<T> LoadAsync(string aggregateId)
        {
            var events = await _eventStore.GetEventsAsync(aggregateId);
            
            if (!events.Any())
            {
                return null;
            }

            var aggregate = (T)Activator.CreateInstance(typeof(T), aggregateId);
            aggregate.LoadFromHistory(events);
            
            return aggregate;
        }

        public async Task SaveAsync(T aggregate)
        {
            var uncommittedEvents = aggregate.GetUncommittedEvents().ToList();
            
            if (!uncommittedEvents.Any())
                return;

            // Calculate the version we expect in the store before saving
            var expectedVersionInStore = aggregate.Version - uncommittedEvents.Count;

            await _eventStore.SaveEventsAsync(
                aggregate.Id,
                uncommittedEvents,
                expectedVersionInStore);

            aggregate.MarkEventsAsCommitted();
        }

        public async Task<bool> ExistsAsync(string aggregateId)
        {
            return await _eventStore.AggregateExistsAsync(aggregateId);
        }
    }
}
