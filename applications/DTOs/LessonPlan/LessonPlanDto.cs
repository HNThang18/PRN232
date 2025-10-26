namespace applications.DTOs.LessonPlan
{
    public class LessonPlanDto
    {
        public int LessonPlanId { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public int LevelId { get; set; }
        public string? LevelName { get; set; }
        public string Title { get; set; }
        public string TopicName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}