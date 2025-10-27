using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.Request.AI
{
    public class AiQuestionRequestDto
    {
        [Required(ErrorMessage = "Topic is required")]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade level is required")]
        [Range(1, 12, ErrorMessage = "Grade level must be between 1 and 12")]
        public int GradeLevel { get; set; }

        [Required(ErrorMessage = "Question type is required")]
        [RegularExpression("^(multiple_choice|true_false|short_answer|essay)$", 
            ErrorMessage = "Invalid question type")]
        public string QuestionType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Difficulty is required")]
        [RegularExpression("^(easy|medium|hard)$", ErrorMessage = "Difficulty must be easy, medium, or hard")]
        public string Difficulty { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Count must be between 1 and 20")]
        public int Count { get; set; } = 1;

        public bool IncludeSolution { get; set; } = true;

        public int? QuestionBankId { get; set; }
        public int? UserId { get; set; }
        public int? LevelId { get; set; }
        public int? DifficultyId { get; set; }
    }
}
