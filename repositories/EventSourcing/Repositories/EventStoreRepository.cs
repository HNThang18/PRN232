using Microsoft.EntityFrameworkCore;
using repositories.Dbcontext;
using repositories.EventSourcing.Events;
using repositories.EventSourcing.Models;
using System.Text.Json;

namespace repositories.EventSourcing.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly MathLpContext _context;
        private readonly JsonSerializerOptions _jsonOptions;

        public EventStoreRepository(MathLpContext context)
        {
            _context = context;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        public async Task SaveEventsAsync(string aggregateId, IEnumerable<IEvent> events, int expectedVersion)
        {
            var eventsList = events.ToList();
            if (!eventsList.Any())
                return;

            // Check for concurrency conflicts
            var currentVersion = await GetLatestVersionAsync(aggregateId);
            if (currentVersion != expectedVersion)
            {
                throw new InvalidOperationException(
                    $"Concurrency conflict for aggregate {aggregateId}. Expected version {expectedVersion} but found {currentVersion}");
            }

            var entities = new List<EventStoreEntity>();
            int version = expectedVersion + 1;

            foreach (var @event in eventsList)
            {
                @event.Version = version;

                var entity = new EventStoreEntity
                {
                    EventId = @event.EventId,
                    AggregateId = aggregateId,
                    EventType = @event.EventType,
                    EventData = JsonSerializer.Serialize(@event, @event.GetType(), _jsonOptions),
                    Version = version,
                    OccurredAt = @event.OccurredAt,
                    UserId = @event.UserId,
                    AggregateType = "QuizGeneration",
                    CreatedAt = DateTime.UtcNow
                };

                entities.Add(entity);
                version++;
            }

            await _context.EventStore.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<IEvent>> GetEventsAsync(string aggregateId)
        {
            var entities = await _context.EventStore
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.Version)
                .ToListAsync();

            return entities.Select(DeserializeEvent);
        }

        public async Task<IEnumerable<IEvent>> GetEventsAsync(string aggregateId, int fromVersion)
        {
            var entities = await _context.EventStore
                .Where(e => e.AggregateId == aggregateId && e.Version >= fromVersion)
                .OrderBy(e => e.Version)
                .ToListAsync();

            return entities.Select(DeserializeEvent);
        }

        public async Task<bool> AggregateExistsAsync(string aggregateId)
        {
            return await _context.EventStore
                .AnyAsync(e => e.AggregateId == aggregateId);
        }

        public async Task<int> GetLatestVersionAsync(string aggregateId)
        {
            var latestVersion = await _context.EventStore
                .Where(e => e.AggregateId == aggregateId)
                .MaxAsync(e => (int?)e.Version);

            return latestVersion ?? -1;
        }

        private IEvent DeserializeEvent(EventStoreEntity entity)
        {
            // Map event type string to actual type
            var eventType = Type.GetType($"repositories.EventSourcing.Events.{entity.EventType}, repositories");
            
            if (eventType == null)
            {
                throw new InvalidOperationException($"Unknown event type: {entity.EventType}");
            }

            var @event = JsonSerializer.Deserialize(entity.EventData, eventType, _jsonOptions) as IEvent;
            
            if (@event == null)
            {
                throw new InvalidOperationException($"Failed to deserialize event: {entity.EventType}");
            }

            return @event;
        }
    }
}
