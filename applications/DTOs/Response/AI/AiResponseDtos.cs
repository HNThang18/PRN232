using System.Text.Json.Serialization;

namespace applications.DTOs.Response.AI
{
    // Lesson Plan Response Models
    public class AiLessonPlanResponseDto
    {
        public string Topic { get; set; } = string.Empty;
        
        [JsonPropertyName("grade_level")]
        public string GradeLevel { get; set; } = string.Empty;
        
        public int Duration { get; set; }
        public List<string> Objectives { get; set; } = new();
        public List<string> Materials { get; set; } = new();
        public List<LessonSectionDto> Sections { get; set; } = new();
        public string Assessment { get; set; } = string.Empty;
        public string? Homework { get; set; }
        public string? Notes { get; set; }
    }

    public class LessonSectionDto
    {
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> Activities { get; set; } = new();
    }

    // Question Response Models
    public class AiQuestionResponseDto
    {
        public List<AiQuestionDto> Questions { get; set; } = new();
        public string Topic { get; set; } = string.Empty;
        
        [JsonPropertyName("grade_level")]
        public string GradeLevel { get; set; } = string.Empty;
    }

    public class AiQuestionDto
    {
        [JsonPropertyName("question_text")]
        public string QuestionText { get; set; } = string.Empty;
        
        [JsonPropertyName("question_type")]
        public string QuestionType { get; set; } = string.Empty;
        
        public List<AiChoiceDto>? Choices { get; set; }
        
        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; } = string.Empty;
        
        public string? Solution { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }

    public class AiChoiceDto
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        
        [JsonPropertyName("is_correct")]
        public bool IsCorrect { get; set; }
    }

    // Quiz Response Models
    public class AiQuizResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        
        [JsonPropertyName("grade_level")]
        public string GradeLevel { get; set; } = string.Empty;
        
        public int Duration { get; set; }
        
        [JsonPropertyName("total_points")]
        public int TotalPoints { get; set; }
        
        public List<AiQuizQuestionDto> Questions { get; set; } = new();
        public string Instructions { get; set; } = string.Empty;
    }

    public class AiQuizQuestionDto
    {
        [JsonPropertyName("question_text")]
        public string QuestionText { get; set; } = string.Empty;
        
        [JsonPropertyName("question_type")]
        public string QuestionType { get; set; } = string.Empty;
        
        public List<AiChoiceDto>? Choices { get; set; }
        
        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; } = string.Empty;
        
        public string Solution { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int Points { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
