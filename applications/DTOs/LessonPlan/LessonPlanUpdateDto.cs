using System.ComponentModel.DataAnnotations;
namespace applications.DTOs.LessonPlan
{
    public class LessonPlanUpdateDto
    {
        [Required]
        public int TeacherId { get; set; }

        [Required]
        public int LevelId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? TopicName { get; set; }

        public string? Description { get; set; }
    }
}