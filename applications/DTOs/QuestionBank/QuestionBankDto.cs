namespace applications.DTOs.QuestionBank
{
    public class QuestionBankDto
    {
        public int QuestionBankId { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public int LevelId { get; set; }
        public string? LevelName { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
    }
}
