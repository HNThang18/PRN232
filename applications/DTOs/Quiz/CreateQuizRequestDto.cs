using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.Quiz
{
    public class CreateQuizRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "LevelId is required")]
        public int LevelId { get; set; }

        [Range(1, 300, ErrorMessage = "TimeLimit must be between 1 and 300 minutes")]
        public int TimeLimit { get; set; }

        [Range(1, 10, ErrorMessage = "AttemptLimit must be between 1 and 10")]
        public int AttemptLimit { get; set; } = 1;

        public List<int>? QuestionIds { get; set; }
    }
}
