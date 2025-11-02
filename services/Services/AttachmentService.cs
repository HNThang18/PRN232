using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;

namespace services.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ILessonDetailRepository _lessonDetailRepository;

        public AttachmentService(
            IAttachmentRepository attachmentRepository,
            ILessonDetailRepository lessonDetailRepository)
        {
            _attachmentRepository = attachmentRepository;
            _lessonDetailRepository = lessonDetailRepository;
        }

        // ================== BASIC CRUD ==================

        public async Task<IEnumerable<Attachment>> GetAllAttachmentsAsync()
        {
            return await _attachmentRepository.GetAllAttachmentsAsync();
        }

        public async Task<Attachment?> GetAttachmentByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID phải lớn hơn 0", nameof(id));

            var attachment = await _attachmentRepository.GetAttachmentByIdAsync(id);
            if (attachment == null)
                throw new KeyNotFoundException($"Không tìm thấy attachment với ID {id}");

            return attachment;
        }

        public async Task<Attachment> AddAttachmentAsync(Attachment attachment)
        {
            // ===== VALIDATION =====

            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));

            if (string.IsNullOrWhiteSpace(attachment.FileName))
                throw new ArgumentException("Tên file không được để trống");

            if (string.IsNullOrWhiteSpace(attachment.FilePath))
                throw new ArgumentException("Đường dẫn file không được để trống");

            if (attachment.LessonDetailId <= 0)
                throw new ArgumentException("LessonDetailId không hợp lệ");

            // Check LessonDetail exists
            var lessonDetailExists = await _lessonDetailRepository.LessonDetailExistsAsync(attachment.LessonDetailId);
            if (!lessonDetailExists)
                throw new KeyNotFoundException($"Không tìm thấy LessonDetail với ID {attachment.LessonDetailId}");

            // Validate file size (max 10MB default)
            if (!await ValidateFileSize(attachment.FileSize))
                throw new InvalidOperationException($"Kích thước file vượt quá giới hạn cho phép (10MB). File size: {await GetFormattedFileSizeAsync(attachment.FileSize)}");

            // Set upload timestamp
            attachment.UploadTimestamp = DateTime.UtcNow;

            await _attachmentRepository.AddAttachmentAsync(attachment);

            return attachment;
        }

        public async Task UpdateAttachmentAsync(Attachment attachment)
        {
            // ===== VALIDATION =====

            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment));

            if (attachment.AttachmentId <= 0)
                throw new ArgumentException("AttachmentId không hợp lệ");

            if (string.IsNullOrWhiteSpace(attachment.FileName))
                throw new ArgumentException("Tên file không được để trống");

            // Check exists
            var exists = await _attachmentRepository.AttachmentExistsAsync(attachment.AttachmentId);
            if (!exists)
                throw new KeyNotFoundException($"Không tìm thấy attachment với ID {attachment.AttachmentId}");

            await _attachmentRepository.UpdateAttachmentAsync(attachment);
        }

        public async Task DeleteAttachmentAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID phải lớn hơn 0", nameof(id));

            // Check exists
            var exists = await _attachmentRepository.AttachmentExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException($"Không tìm thấy attachment với ID {id}");

            // TODO: Xóa file vật lý trên server/cloud storage
            // var attachment = await _attachmentRepository.GetAttachmentByIdAsync(id);
            // await DeletePhysicalFileAsync(attachment.FilePath);

            await _attachmentRepository.DeleteAttachmentAsync(id);
        }

        // ================== QUERY METHODS ==================

        public async Task<IEnumerable<Attachment>> GetAttachmentsByLessonDetailIdAsync(int lessonDetailId)
        {
            if (lessonDetailId <= 0)
                throw new ArgumentException("LessonDetailId phải lớn hơn 0", nameof(lessonDetailId));

            return await _attachmentRepository.GetAttachmentsByLessonDetailIdAsync(lessonDetailId);
        }

        public async Task<IEnumerable<Attachment>> GetAttachmentsByFileTypeAsync(string fileType)
        {
            return await _attachmentRepository.GetAttachmentsByFileTypeAsync(fileType);
        }

        public async Task<IEnumerable<Attachment>> GetAttachmentsByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId phải lớn hơn 0", nameof(userId));

            return await _attachmentRepository.GetAttachmentsByUserIdAsync(userId);
        }

        /// <summary>
        /// Đếm số lượng attachments theo LessonDetailId
        /// </summary>
        public async Task<int> CountAttachmentsByLessonDetailIdAsync(int lessonDetailId)
        {
            if (lessonDetailId <= 0)
                throw new ArgumentException("LessonDetailId phải lớn hơn 0", nameof(lessonDetailId));

            return await _attachmentRepository.CountAttachmentsByLessonDetailIdAsync(lessonDetailId);
        }

        // ================== BUSINESS METHODS ==================

        /// <summary>
        /// Tính tổng dung lượng files của một lesson detail
        /// </summary>
        public async Task<long> GetTotalFileSizeByLessonDetailIdAsync(int lessonDetailId)
        {
            if (lessonDetailId <= 0)
                throw new ArgumentException("LessonDetailId phải lớn hơn 0", nameof(lessonDetailId));

            return await _attachmentRepository.GetTotalFileSizeByLessonDetailIdAsync(lessonDetailId);
        }

        /// <summary>
        /// Tính tổng dung lượng files được upload bởi một user
        /// </summary>
        public async Task<long> GetTotalFileSizeByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId phải lớn hơn 0", nameof(userId));

            return await _attachmentRepository.GetTotalFileSizeByUserIdAsync(userId);
        }

        /// <summary>
        /// Format file size thành dạng human-readable (KB, MB, GB)
        /// </summary>
        public Task<string> GetFormattedFileSizeAsync(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return Task.FromResult($"{len:0.##} {sizes[order]}");
        }

        /// <summary>
        /// Validate file size (default max 10MB)
        /// </summary>
        public Task<bool> ValidateFileSize(long fileSize, long maxSize = 10485760)
        {
            if (fileSize <= 0)
                return Task.FromResult(false);

            if (fileSize > maxSize)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Validate file type
        /// </summary>
        public Task<bool> ValidateFileType(string fileType, string[] allowedTypes)
        {
            if (string.IsNullOrWhiteSpace(fileType))
                return Task.FromResult(false);

            if (allowedTypes == null || allowedTypes.Length == 0)
                return Task.FromResult(true);

            var normalizedFileType = fileType.ToLower().Trim();
            return Task.FromResult(allowedTypes.Any(t => normalizedFileType.Contains(t.ToLower())));
        }

        /// <summary>
        /// Xóa tất cả attachments của một lesson detail
        /// </summary>
        public async Task DeleteAttachmentsByLessonDetailIdAsync(int lessonDetailId)
        {
            if (lessonDetailId <= 0)
                throw new ArgumentException("LessonDetailId phải lớn hơn 0", nameof(lessonDetailId));

            var attachments = await _attachmentRepository.GetAttachmentsByLessonDetailIdAsync(lessonDetailId);

            if (attachments != null && attachments.Any())
            {
                // TODO: Xóa các files vật lý
                // foreach (var attachment in attachments)
                // {
                //     await DeletePhysicalFileAsync(attachment.FilePath);
                // }

                await _attachmentRepository.DeleteRangeAsync(attachments);
            }
        }

        // TODO: Implement physical file deletion
        // private async Task DeletePhysicalFileAsync(string filePath)
        // {
        //     // Implementation for deleting file from server/cloud storage
        // }
    }
}
