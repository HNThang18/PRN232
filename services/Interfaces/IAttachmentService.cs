using repositories.Models;

namespace services.Interfaces
{
    public interface IAttachmentService
    {
        // Basic CRUD
        Task<IEnumerable<Attachment>> GetAllAttachmentsAsync();
        Task<Attachment?> GetAttachmentByIdAsync(int id);
        Task<Attachment> AddAttachmentAsync(Attachment attachment);
        Task UpdateAttachmentAsync(Attachment attachment);
        Task DeleteAttachmentAsync(int id);

        // Query methods
        Task<IEnumerable<Attachment>> GetAttachmentsByLessonDetailIdAsync(int lessonDetailId);
        Task<IEnumerable<Attachment>> GetAttachmentsByFileTypeAsync(string fileType);
        Task<IEnumerable<Attachment>> GetAttachmentsByUserIdAsync(int userId);
        Task<int> CountAttachmentsByLessonDetailIdAsync(int lessonDetailId);

        // Business methods
        Task<long> GetTotalFileSizeByLessonDetailIdAsync(int lessonDetailId);
        Task<long> GetTotalFileSizeByUserIdAsync(int userId);
        Task<string> GetFormattedFileSizeAsync(long bytes);
        Task<bool> ValidateFileSize(long fileSize, long maxSize = 10485760); // 10MB default
        Task<bool> ValidateFileType(string fileType, string[] allowedTypes);
        Task DeleteAttachmentsByLessonDetailIdAsync(int lessonDetailId);
    }
}
