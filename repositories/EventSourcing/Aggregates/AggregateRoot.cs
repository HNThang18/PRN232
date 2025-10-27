using repositories.EventSourcing.Events;

namespace repositories.EventSourcing.Aggregates
{
    public abstract class AggregateRoot
    {
        private readonly List<IEvent> _uncommittedEvents = new();

        public string Id { get; protected set; }
        public int Version { get; protected set; }

        protected AggregateRoot(string id)
        {
            Id = id;
            Version = -1;
        }

        public IEnumerable<IEvent> GetUncommittedEvents()
        {
            return _uncommittedEvents.AsReadOnly();
        }

        public void MarkEventsAsCommitted()
        {
            _uncommittedEvents.Clear();
        }

        protected void ApplyEvent(IEvent @event)
        {
            ApplyEvent(@event, true);
        }

        /// <param name="event">Event to apply</param>
        /// <param name="isNew">Whether this is a new event or replaying from store</param>
        private void ApplyEvent(IEvent @event, bool isNew)
        {
            // Use reflection to call the appropriate Apply method
            var eventType = @event.GetType();
            var method = GetType().GetMethod("Apply", new[] { eventType });
            
            if (method == null)
            {
                throw new InvalidOperationException(
                    $"No Apply method found for event type {eventType.Name} in aggregate {GetType().Name}");
            }

            method.Invoke(this, new object[] { @event });

            if (isNew)
            {
                _uncommittedEvents.Add(@event);
                Version++;
            }
            else
            {
                Version = @event.Version;
            }
        }

        public void LoadFromHistory(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                ApplyEvent(@event, false);
            }
        }

        internal void UpdateVersionAfterSave(int newVersion)
        {
            Version = newVersion;
        }
    }
}
