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
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required]
        [ForeignKey("LessonPlan")]
        public int LessonPlanId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string? Content { get; set; }

        public DateTime? PublishedDate { get; set; }

        public bool IsShared { get; set; } = false;

        public virtual LessonPlan LessonPlan { get; set; }
        public virtual ICollection<LessonDetail> LessonDetails { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
    }
}
