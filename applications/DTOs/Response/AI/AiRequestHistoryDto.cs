namespace applications.DTOs.Response.AI
{
    public class AiRequestHistoryDto
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public string? Response { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; } = string.Empty;
    }

    public class AiRequestDetailDto : AiRequestHistoryDto
    {
        public List<int>? LessonPlanIds { get; set; }
        public List<int>? QuestionIds { get; set; }
    }
}
