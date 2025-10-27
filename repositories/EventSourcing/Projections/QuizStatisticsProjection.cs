using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace repositories.EventSourcing.Projections
{
    [Table("QuizStatisticsProjection")]
    public class QuizStatisticsProjection
    {
        [Key]
        public int Id { get; set; }

        public int TotalInitiated { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalFailed { get; set; }
        public double SuccessRate { get; set; }

        public double AverageDuration { get; set; }
        public double MinDuration { get; set; }
        public double MaxDuration { get; set; }

        public int TotalQuestionsGenerated { get; set; }
        public double AverageQuestionsPerQuiz { get; set; }

        public string GradeLevelDistribution { get; set; } = "{}";

        public string TopTopics { get; set; } = "[]";

        public DateTime LastUpdated { get; set; }
    }
}
