namespace applications.DTOs.Request
{
    public class QuestionRequestDto
    {
        public int? QuizId { get; set; }
        public int? QuestionBankId { get; set; }
        public int? DifficultyId { get; set; }
        public int Topic { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int QuestionType { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public string? Tags { get; set; }
        public bool IsAIGenerated { get; set; }
        public int Status { get; set; }
    }
}