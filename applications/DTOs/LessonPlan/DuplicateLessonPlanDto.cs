using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.LessonPlan
{
    public class DuplicateLessonPlanDto
    {
        [Required(ErrorMessage = "NewTeacherId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "NewTeacherId must be greater than 0")]
        public int NewTeacherId { get; set; }
    }
}
