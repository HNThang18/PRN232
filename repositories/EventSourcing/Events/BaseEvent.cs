namespace repositories.EventSourcing.Events
{
    public abstract class BaseEvent : IEvent
    {
        public Guid EventId { get; protected set; }
        public string EventType { get; protected set; }
        public DateTime OccurredAt { get; protected set; }
        public string AggregateId { get; protected set; }
        public int Version { get; set; }
        public int? UserId { get; protected set; }

        protected BaseEvent(string aggregateId, int? userId = null)
        {
            EventId = Guid.NewGuid();
            EventType = GetType().Name;
            OccurredAt = DateTime.UtcNow;
            AggregateId = aggregateId;
            UserId = userId;
        }

        // For deserialization
        protected BaseEvent()
        {
            EventType = GetType().Name;
        }
    }
}
