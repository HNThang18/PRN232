using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace repositories.EventSourcing.Projections
{
    [Table("QuizListProjection")]
    public class QuizListProjection
    {
        [Key]
        public string AggregateId { get; set; } = string.Empty;

        public int? QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public int GradeLevel { get; set; }

        public int TeacherId { get; set; }
        public int LevelId { get; set; }

        public int QuestionCount { get; set; }
        public int TotalScore { get; set; }
        public int Duration { get; set; }

        public string Status { get; set; } = "Processing"; // "Processing", "Completed", "Failed"
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }
        public string? ErrorMessage { get; set; }

        public int? AiRequestId { get; set; }

        public DateTime InitiatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public double? ProcessingDuration { get; set; }

        public DateTime LastUpdated { get; set; }
        public int Version { get; set; }
    }
}
