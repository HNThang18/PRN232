using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.Question
{
    public class UpdateQuestionRequestDto
    {
        public int? QuestionBankId { get; set; }

        public int? DifficultyId { get; set; }

        public string? Topic { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public string? QuestionType { get; set; }

        public string? CorrectAnswer { get; set; }

        public string? Explanation { get; set; }

        public string? Tags { get; set; }

        public string? Status { get; set; }

        public List<UpdateAnswerDto>? Answers { get; set; }
    }

    public class UpdateAnswerDto
    {
        public int? AnswerId { get; set; } // null for new answers

        [Required]
        public string AnswerText { get; set; } = string.Empty;

        [Required]
        public bool IsCorrect { get; set; }
    }
}
