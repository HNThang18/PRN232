using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace repositories.EventSourcing.Models
{
    [Table("EventStore")]
    public class EventStoreEntity
    {
        [Key]
        public Guid EventId { get; set; }

        [Required]
        [MaxLength(200)]
        public string AggregateId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventType { get; set; }

        [Required]
        public string EventData { get; set; }

        public int Version { get; set; }

        public DateTime OccurredAt { get; set; }

        public int? UserId { get; set; }

        [MaxLength(50)]
        public string AggregateType { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
