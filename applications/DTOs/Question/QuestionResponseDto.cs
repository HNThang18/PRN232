namespace applications.DTOs.Question
{
    public class QuestionResponseDto
    {
        public int QuestionId { get; set; }
        public string Topic { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public string? Tags { get; set; }
        public bool IsAIGenerated { get; set; }
        public string Status { get; set; }
        public int? DifficultyId { get; set; }
        public string? DifficultyName { get; set; }
        public List<AnswerResponseDto> Answers { get; set; }
    }

    public class AnswerResponseDto
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
