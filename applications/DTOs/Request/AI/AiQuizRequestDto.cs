using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace applications.DTOs.Request.AI
{
    public class DifficultyDistribution
    {
        [JsonPropertyName("easy")]
        [Range(0, 100, ErrorMessage = "Easy percentage must be between 0 and 100")]
        public double Easy { get; set; } = 0;

        [JsonPropertyName("medium")]
        [Range(0, 100, ErrorMessage = "Medium percentage must be between 0 and 100")]
        public double Medium { get; set; } = 0;

        [JsonPropertyName("hard")]
        [Range(0, 100, ErrorMessage = "Hard percentage must be between 0 and 100")]
        public double Hard { get; set; } = 0;
    }

    public class AiQuizRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Topic is required")]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade level is required")]
        [Range(1, 12, ErrorMessage = "Grade level must be between 1 and 12")]
        public int GradeLevel { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(10, 120, ErrorMessage = "Duration must be between 10 and 120 minutes")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Question count is required")]
        [Range(5, 50, ErrorMessage = "Question count must be between 5 and 50")]
        public int QuestionCount { get; set; }

        [JsonPropertyName("difficultyDistribution")]
        public DifficultyDistribution? DifficultyDistribution { get; set; }

        public bool IncludeEssay { get; set; } = false;

        // ASP.NET specific fields
        public int? TeacherId { get; set; }
        public int? QuestionBankId { get; set; }
    }
}
