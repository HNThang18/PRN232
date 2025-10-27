namespace applications.DTOs.Response
{
    public class LessonResponseDto
    {
        public int LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public int Order { get; set; }
        public bool IsShared { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys only
        public int LessonPlanId { get; set; }

        // Optional: Include minimal related data
        public string? LessonPlanTitle { get; set; }

        // Optional: Include counts
        public int LessonDetailsCount { get; set; }
        public int ProgressCount { get; set; }
    }

    /// <summary>
    /// Extended version with lesson details
    /// </summary>
    public class LessonWithDetailsDto : LessonResponseDto
    {
        public List<LessonDetailResponseDto> LessonDetails { get; set; } = new();
    }
}
