namespace applications.DTOs.Request
{
    public class AnswerRequestDto
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}