namespace applications.DTOs.Response
{
    public class LessonPlanResponseDto
    {
        public int LessonPlanId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int EstimatedDuration { get; set; }
        public string Status { get; set; } = string.Empty; // "Draft", "Published", "Archived", "Deleted"
        public bool IsPublic { get; set; }
        public int Version { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys only
        public int TeacherId { get; set; }
        public int LevelId { get; set; }
        public int? AiRequestId { get; set; }

        // Optional: Include minimal related data
        public string? TeacherName { get; set; }
        public string? LevelName { get; set; }
        
        // Optional: Include counts
        public int LessonsCount { get; set; }
    }

    /// <summary>
    /// Extended version with lessons
    /// </summary>
    public class LessonPlanWithLessonsDto : LessonPlanResponseDto
    {
        public List<LessonResponseDto> Lessons { get; set; } = new();
    }
}
