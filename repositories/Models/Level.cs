using Azure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public enum EducationLevel
    {
        PrimarySchool,
        SecondarySchool,
        HighSchool
    }
    public class Level
    {
        [Key]
        public int LevelId { get; set; }

        [Required]
        [MaxLength(100)]
        public string LevelName { get; set; }

        [Required]
        public EducationLevel EducationLevel { get; set; }

        public int Order { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<LessonPlan> LessonPlans { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
        public virtual ICollection<AIRequest> AIRequests { get; set; }
        public virtual ICollection<QuestionBank> QuestionBanks { get; set; }
    }
}
