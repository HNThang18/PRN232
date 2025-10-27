using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.Request.AI
{
    public class AiLessonPlanRequestDto
    {
        [Required(ErrorMessage = "Topic is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Topic must be between 3 and 200 characters")]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade level is required")]
        [RegularExpression("^(elementary|middle|high)$", ErrorMessage = "Grade level must be elementary, middle, or high")]
        public string GradeLevel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required")]
        [Range(30, 90, ErrorMessage = "Duration must be between 30 and 90 minutes")]
        public int Duration { get; set; }

        public List<string>? Objectives { get; set; }

        public string? AdditionalRequirements { get; set; }

        // ASP.NET specific fields
        [Required(ErrorMessage = "Teacher ID is required")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Level ID is required")]
        public int LevelId { get; set; }

        [Range(1, 12, ErrorMessage = "Grade must be between 1 and 12")]
        public int Grade { get; set; }
    }
}
