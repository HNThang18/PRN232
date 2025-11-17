using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.Quiz
{
    public class UpdateQuizRequestDto
    {
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [Range(1, 300, ErrorMessage = "TimeLimit must be between 1 and 300 minutes")]
        public int? TimeLimit { get; set; }

        [Range(1, 10, ErrorMessage = "AttemptLimit must be between 1 and 10")]
        public int? AttemptLimit { get; set; }

        public List<int>? QuestionIds { get; set; }
    }
}
