using Azure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public enum LessonPlanStatus
    {
        Draft,
        Published,
        Deleted
    }
    public class LessonPlan
    {
        [Key]
        public int LessonPlanId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int TeacherId { get; set; }

        [Required]
        [ForeignKey("Level")]
        public int LevelId { get; set; }

        [ForeignKey("AIRequest")]
        public int? AIRequestId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public int Duration { get; set; } // Thời lượng tính bằng phút

        public string? Content { get; set; }

        [Required]
        public LessonPlanStatus Status { get; set; }

        public int Version { get; set; } = 1;

        public string? ExportPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        public virtual User Teacher { get; set; }
        public virtual Level Level { get; set; }
        public virtual AIRequest AIRequest { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
