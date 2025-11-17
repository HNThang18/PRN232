namespace applications.DTOs.Quiz
{
    public class QuizStatisticsDto
    {
        public int QuizId { get; set; }
        public string Title { get; set; }
        public int TotalSubmissions { get; set; }
        public int CompletedSubmissions { get; set; }
        public int InProgressSubmissions { get; set; }
        public decimal AverageScore { get; set; }
        public decimal HighestScore { get; set; }
        public decimal LowestScore { get; set; }
        public double AverageDuration { get; set; }
        public int TotalStudents { get; set; }
    }
}
