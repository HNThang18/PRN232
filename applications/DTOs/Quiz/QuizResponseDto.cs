namespace applications.DTOs.Quiz
{
    public class QuizResponseDto
    {
        public int QuizId { get; set; }
        public string Title { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int TimeLimit { get; set; }
        public int AttemptLimit { get; set; }
        public int TotalScore { get; set; }
        public bool IsAIGenerated { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int QuestionCount { get; set; }
        public int SubmissionCount { get; set; }
    }
}
