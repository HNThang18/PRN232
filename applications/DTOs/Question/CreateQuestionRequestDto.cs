using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.Question
{
    public class CreateQuestionRequestDto
    {
        [Required]
        public int QuestionBankId { get; set; }

        [Required]
        public int DifficultyId { get; set; }

        [Required]
        public string Topic { get; set; } = string.Empty;

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        public string QuestionType { get; set; } = string.Empty;

        public string? CorrectAnswer { get; set; }

        public string? Explanation { get; set; }

        public string? Tags { get; set; }

        public bool IsAIGenerated { get; set; }

        public List<CreateAnswerDto>? Answers { get; set; }
    }

    public class CreateAnswerDto
    {
        [Required]
        public string AnswerText { get; set; } = string.Empty;

        [Required]
        public bool IsCorrect { get; set; }
    }
}
