using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;

namespace repositories.Repositories
{
    public class LessonDetailRepository : GenericRepository<LessonDetail>, ILessonDetailRepository
    {
        public LessonDetailRepository(MathLpContext context) : base(context)
        {
        }

        // ===== BASIC CRUD =====

        public async Task<IEnumerable<LessonDetail>> GetAllLessonDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.lessonDetails
                .AsNoTracking()
                .OrderBy(ld => ld.LessonId)
                .ThenBy(ld => ld.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<LessonDetail?> GetLessonDetailByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.lessonDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(ld => ld.LessonDetailId == id, cancellationToken);
        }

        public async Task<LessonDetail?> GetLessonDetailWithAttachmentsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.lessonDetails
                .AsNoTracking()
                .Include(ld => ld.Attachments)
                .Include(ld => ld.Lesson)
                .FirstOrDefaultAsync(ld => ld.LessonDetailId == id, cancellationToken);
        }

        public async Task AddLessonDetailAsync(LessonDetail lessonDetail, CancellationToken cancellationToken = default)
        {
            await _context.lessonDetails.AddAsync(lessonDetail, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateLessonDetailAsync(LessonDetail lessonDetail, CancellationToken cancellationToken = default)
        {
            _context.lessonDetails.Update(lessonDetail);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteLessonDetailAsync(int id, CancellationToken cancellationToken = default)
        {
            var lessonDetail = await _context.lessonDetails.FindAsync(new object[] { id }, cancellationToken);
            if (lessonDetail != null)
            {
                _context.lessonDetails.Remove(lessonDetail);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> LessonDetailExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.lessonDetails
                .AnyAsync(ld => ld.LessonDetailId == id, cancellationToken);
        }

        // ===== QUERY METHODS =====

        public async Task<IEnumerable<LessonDetail>> GetLessonDetailsByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            return await _context.lessonDetails
                .AsNoTracking()
                .Where(ld => ld.LessonId == lessonId)
                .OrderBy(ld => ld.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<LessonDetail>> GetLessonDetailsByContentTypeAsync(ContentType contentType, CancellationToken cancellationToken = default)
        {
            return await _context.lessonDetails
                .AsNoTracking()
                .Where(ld => ld.ContentType == contentType)
                .OrderBy(ld => ld.LessonId)
                .ThenBy(ld => ld.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountLessonDetailsByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            return await _context.lessonDetails
                .CountAsync(ld => ld.LessonId == lessonId, cancellationToken);
        }

        public async Task<int> GetMaxOrderByLessonIdAsync(int lessonId, CancellationToken cancellationToken = default)
        {
            var maxOrder = await _context.lessonDetails
                .Where(ld => ld.LessonId == lessonId)
                .MaxAsync(ld => (int?)ld.Order, cancellationToken);

            return maxOrder ?? 0;
        }

        public async Task<bool> IsOrderExistsAsync(int lessonId, int order, int? excludeLessonDetailId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.lessonDetails
                .Where(ld => ld.LessonId == lessonId && ld.Order == order);

            if (excludeLessonDetailId.HasValue)
            {
                query = query.Where(ld => ld.LessonDetailId != excludeLessonDetailId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        // ===== BUSINESS METHODS =====

        public async Task<bool> HasAttachmentsAsync(int lessonDetailId, CancellationToken cancellationToken = default)
        {
            return await _context.attachments
                .AnyAsync(a => a.LessonDetailId == lessonDetailId, cancellationToken);
        }

        // ===== BULK OPERATIONS =====

        public async Task UpdateRangeAsync(IEnumerable<LessonDetail> lessonDetails, CancellationToken cancellationToken = default)
        {
            _context.lessonDetails.UpdateRange(lessonDetails);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRangeAsync(IEnumerable<LessonDetail> lessonDetails, CancellationToken cancellationToken = default)
        {
            _context.lessonDetails.RemoveRange(lessonDetails);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
