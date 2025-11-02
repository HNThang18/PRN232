namespace applications.DTOs.Response
{
    public class AttachmentResponseDto
    {
        public int AttachmentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FormattedFileSize { get; set; } = string.Empty;
        public DateTime UploadTimestamp { get; set; }

        // Foreign keys only (no navigation properties)
        public int LessonDetailId { get; set; }
        public int? UploadedBy { get; set; }

        // Optional: Include minimal related data
        public string? LessonDetailContentType { get; set; }
    }
}
