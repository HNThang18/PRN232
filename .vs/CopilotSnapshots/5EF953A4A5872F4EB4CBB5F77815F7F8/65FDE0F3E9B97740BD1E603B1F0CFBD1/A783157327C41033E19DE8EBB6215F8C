using Azure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public enum UserRole
    {
        Student,
        Teacher,
        Admin
    }
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Level")]
        public int? LevelId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } // Sẽ được hash trước khi lưu

        public decimal Credit { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [MaxLength(50)]
        public string? GradeLevel { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Level Level { get; set; }
        public virtual ICollection<LessonPlan> LessonPlans { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
        public virtual ICollection<AiRequest> AiRequests { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
        public virtual ICollection<QuestionBank> QuestionBanks { get; set; }
    }
}
