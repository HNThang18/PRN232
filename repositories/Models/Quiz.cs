using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public enum QuizStatus
    {
        Draft,
        Published,
        Deleted
    }
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int TeacherId { get; set; }

        [Required]
        [ForeignKey("Level")]
        public int LevelId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public int TimeLimit { get; set; } // Giới hạn thời gian (phút)
        public int AttemptLimit { get; set; }
        public int TotalScore { get; set; }
        public bool IsAIGenerated { get; set; } = false;
        public QuizStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAt { get; set; }

        public virtual User Teacher { get; set; }
        public virtual Level Level { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
