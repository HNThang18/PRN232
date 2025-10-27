using repositories.Models;

namespace repositories.Interfaces
{
    public interface IAttachmentRepository
    {
        // Basic CRUD
        Task<IEnumerable<Attachment>> GetAllAttachmentsAsync();
        Task<Attachment?> GetAttachmentByIdAsync(int id);
        Task AddAttachmentAsync(Attachment attachment);
        Task UpdateAttachmentAsync(Attachment attachment);
        Task DeleteAttachmentAsync(int id);
        Task<bool> AttachmentExistsAsync(int id);

        // Query methods
        Task<IEnumerable<Attachment>> GetAttachmentsByLessonDetailIdAsync(int lessonDetailId);
        Task<IEnumerable<Attachment>> GetAttachmentsByFileTypeAsync(string fileType);
        Task<IEnumerable<Attachment>> GetAttachmentsByUserIdAsync(int userId);
        Task<long> GetTotalFileSizeByLessonDetailIdAsync(int lessonDetailId);
        Task<long> GetTotalFileSizeByUserIdAsync(int userId);
        Task<int> CountAttachmentsByLessonDetailIdAsync(int lessonDetailId);
        Task UpdateRangeAsync(IEnumerable<Attachment> attachments);
        Task DeleteRangeAsync(IEnumerable<Attachment> attachments);
    }
}
