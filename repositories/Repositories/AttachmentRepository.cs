using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;

namespace repositories.Repositories
{
    public class AttachmentRepository : GenericRepository<Attachment>, IAttachmentRepository
    {
        public AttachmentRepository(MathLpContext context) : base(context) { }

        // ================== BASIC CRUD ==================

        /// <summary>
        /// Lấy tất cả attachments
        /// </summary>
        public async Task<IEnumerable<Attachment>> GetAllAttachmentsAsync()
        {
            return await _context.attachments
                .Include(a => a.LessonDetail)
                    .ThenInclude(ld => ld.Lesson)
                .Include(a => a.User)
                .OrderByDescending(a => a.UploadTimestamp)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Lấy attachment theo ID
        /// </summary>
        public async Task<Attachment?> GetAttachmentByIdAsync(int id)
        {
            return await _context.attachments
                .Include(a => a.LessonDetail)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AttachmentId == id);
        }

        /// <summary>
        /// Thêm attachment mới
        /// </summary>
        public async Task AddAttachmentAsync(Attachment attachment)
        {
            if (attachment.UploadTimestamp == default)
                attachment.UploadTimestamp = DateTime.UtcNow;

            await _context.attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Cập nhật attachment
        /// </summary>
        public async Task UpdateAttachmentAsync(Attachment attachment)
        {
            var exists = await AttachmentExistsAsync(attachment.AttachmentId);
            if (!exists)
                throw new KeyNotFoundException($"Attachment với ID {attachment.AttachmentId} không tồn tại");

            _context.Entry(attachment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AttachmentExistsAsync(attachment.AttachmentId))
                    throw new KeyNotFoundException($"Attachment với ID {attachment.AttachmentId} đã bị xóa");
                throw;
            }
        }

        /// <summary>
        /// Xóa attachment
        /// </summary>
        public async Task DeleteAttachmentAsync(int id)
        {
            var attachment = await _context.attachments.FindAsync(id);
            if (attachment == null)
                throw new KeyNotFoundException($"Attachment với ID {id} không tồn tại");

            _context.attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Kiểm tra attachment có tồn tại không
        /// </summary>
        public async Task<bool> AttachmentExistsAsync(int id)
        {
            return await _context.attachments.AnyAsync(a => a.AttachmentId == id);
        }

        // ================== QUERY METHODS ==================

        /// <summary>
        /// Lấy tất cả attachments của một lesson detail
        /// </summary>
        public async Task<IEnumerable<Attachment>> GetAttachmentsByLessonDetailIdAsync(int lessonDetailId)
        {
            return await _context.attachments
                .Include(a => a.User)
                .Where(a => a.LessonDetailId == lessonDetailId)
                .OrderBy(a => a.FileName)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Lấy attachments theo loại file (image, pdf, video...)
        /// </summary>
        public async Task<IEnumerable<Attachment>> GetAttachmentsByFileTypeAsync(string fileType)
        {
            if (string.IsNullOrWhiteSpace(fileType))
                return await GetAllAttachmentsAsync();

            return await _context.attachments
                .Include(a => a.LessonDetail)
                .Include(a => a.User)
                .Where(a => a.FileType.ToLower().Contains(fileType.ToLower()))
                .OrderByDescending(a => a.UploadTimestamp)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Lấy attachments được upload bởi một user
        /// </summary>
        public async Task<IEnumerable<Attachment>> GetAttachmentsByUserIdAsync(int userId)
        {
            return await _context.attachments
                .Include(a => a.LessonDetail)
                .Where(a => a.UploadedBy == userId)
                .OrderByDescending(a => a.UploadTimestamp)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Tính tổng dung lượng files của một lesson detail
        /// </summary>
        public async Task<long> GetTotalFileSizeByLessonDetailIdAsync(int lessonDetailId)
        {
            var totalSize = await _context.attachments
                .Where(a => a.LessonDetailId == lessonDetailId)
                .SumAsync(a => (long?)a.FileSize);

            return totalSize ?? 0;
        }

        /// <summary>
        /// Tính tổng dung lượng files được upload bởi một user
        /// </summary>
        public async Task<long> GetTotalFileSizeByUserIdAsync(int userId)
        {
            var totalSize = await _context.attachments
                .Where(a => a.UploadedBy == userId)
                .SumAsync(a => (long?)a.FileSize);

            return totalSize ?? 0;
        }

        /// <summary>
        /// Đếm số attachments của một lesson detail
        /// </summary>
        public async Task<int> CountAttachmentsByLessonDetailIdAsync(int lessonDetailId)
        {
            return await _context.attachments
                .CountAsync(a => a.LessonDetailId == lessonDetailId);
        }

        /// <summary>
        /// Bulk update attachments
        /// </summary>
        public async Task UpdateRangeAsync(IEnumerable<Attachment> attachments)
        {
            if (attachments == null || !attachments.Any())
                return;

            foreach (var attachment in attachments)
            {
                _context.Entry(attachment).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Bulk delete attachments
        /// </summary>
        public async Task DeleteRangeAsync(IEnumerable<Attachment> attachments)
        {
            if (attachments == null || !attachments.Any())
                return;

            _context.attachments.RemoveRange(attachments);
            await _context.SaveChangesAsync();
        }
    }
}
