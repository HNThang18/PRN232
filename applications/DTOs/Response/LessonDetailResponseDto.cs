namespace applications.DTOs.Response
{
    public class LessonDetailResponseDto
    {
        public int LessonDetailId { get; set; }
        public int Order { get; set; }
        public string ContentType { get; set; } = string.Empty; // "Text", "Video", "Image", "Audio", "Quiz"
        public string Content { get; set; } = string.Empty;
        public string? ContentLaTeX { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys only
        public int LessonId { get; set; }

        // Optional: Include minimal related data
        public string? LessonTitle { get; set; }

        // Optional: Include attachments count instead of full list
        public int AttachmentsCount { get; set; }
    }

    /// <summary>
    /// Extended version with attachments list
    /// </summary>
    public class LessonDetailWithAttachmentsDto : LessonDetailResponseDto
    {
        public List<AttachmentResponseDto> Attachments { get; set; } = new();
    }
}
