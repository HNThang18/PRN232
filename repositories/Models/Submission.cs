using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public enum SubissionStatus
    {
        Completed,
        InProgress,
        NotStarted,
        Late,
        Failed
    }
    public class Submission
    {
        [Key]
        public int SubmissionId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Quiz")]
        public int QuizId { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public SubissionStatus Status { get; set; }

        public decimal Score { get; set; }

        public string? Feedback { get; set; }
        public int AttemptNumber { get; set; }
        public int DurationTaken { get; set; } // Thời gian làm bài (giây)

        public virtual User Student { get; set; }
        public virtual Quiz Quiz { get; set; }
        public virtual ICollection<SubmissionDetail> SubmissionDetails { get; set; }
    }
}
