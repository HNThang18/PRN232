namespace repositories.EventSourcing.Events
{
    public interface IEvent
    {
        Guid EventId { get; }
        string EventType { get; }
        DateTime OccurredAt { get; }
        string AggregateId { get; }
        int Version { get; set; }
        int? UserId { get; }
    }
}
