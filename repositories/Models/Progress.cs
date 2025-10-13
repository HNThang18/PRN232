using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public enum ProgressStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Late
    }
    public class Progress
    {
        [Key]
        public int ProgressId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }

        [MaxLength(50)]
        public ProgressStatus CompletionStatus { get; set; }

        public DateTime? AttemptDate { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual User Student { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}
