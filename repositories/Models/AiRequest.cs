using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public enum AIRequestStatus
    {
        Pending,
        Success,
        Failed
    }
    public enum RequestType
    {
        GenerateLessonPlan,
        GenerateQuiz
    }
    public class AIRequest
    {
        [Key]
        public int AIRequestId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Level")]
        public int LevelId { get; set; }

        [Required]
        [MaxLength(100)]
        public RequestType RequestType { get; set; }

        [Required]
        public string Prompt { get; set; }

        public string? Response { get; set; }

        [MaxLength(50)]
        public AIRequestStatus Status { get; set; }

        public decimal Cost { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }
        public virtual Level Level { get; set; }
        public virtual ICollection<LessonPlan> LessonPlans { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
