using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;

namespace repositories.Repositories
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(MathLpContext context) : base(context) { }

        // ================== BASIC CRUD ==================

        /// <summary>
        /// Lấy tất cả lessons (không include children - dùng cho list)
        /// </summary>
        public async Task<IEnumerable<Lesson>> GetAllLessonsAsync()
        {
            return await _context.lessons
                .Include(l => l.LessonPlan) // Include parent để hiển thị tên giáo án
                .OrderBy(l => l.LessonPlanId)
                .ThenBy(l => l.Order) // Sort theo thứ tự
                .AsNoTracking() // Read-only, tăng performance
                .ToListAsync();
        }

        /// <summary>
        /// Lấy lesson theo ID (không include children - dùng cho edit form)
        /// </summary>
        public async Task<Lesson?> GetLessonByIdAsync(int id)
        {
            return await _context.lessons
                .Include(l => l.LessonPlan) // Include parent
                .FirstOrDefaultAsync(l => l.LessonId == id);
        }

        /// <summary>
        /// Lấy lesson với tất cả details (dùng cho view chi tiết)
        /// </summary>
        public async Task<Lesson?> GetLessonWithDetailsAsync(int id)
        {
            return await _context.lessons
                .Include(l => l.LessonPlan)
                .Include(l => l.LessonDetails.OrderBy(d => d.Order)) // Include + sort details
                    .ThenInclude(d => d.Attachments) // Include attachments của detail
                .Include(l => l.Progresses) // Include progress nếu cần
                .FirstOrDefaultAsync(l => l.LessonId == id);
        }

        /// <summary>
        /// Thêm lesson mới
        /// </summary>
        public async Task AddLessonAsync(Lesson lesson)
        {
            // Set CreatedAt nếu chưa có
            if (lesson.CreatedAt == default)
                lesson.CreatedAt = DateTime.UtcNow;

            await _context.lessons.AddAsync(lesson);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Cập nhật lesson
        /// </summary>
        public async Task UpdateLessonAsync(Lesson lesson)
        {
            // Kiểm tra lesson có tồn tại không
            var exists = await LessonExistsAsync(lesson.LessonId);
            if (!exists)
                throw new KeyNotFoundException($"Lesson với ID {lesson.LessonId} không tồn tại");

            // Set UpdatedAt
            lesson.UpdatedAt = DateTime.UtcNow;

            _context.Entry(lesson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý trường hợp conflict
                if (!await LessonExistsAsync(lesson.LessonId))
                    throw new KeyNotFoundException($"Lesson với ID {lesson.LessonId} đã bị xóa");
                throw;
            }
        }

        /// <summary>
        /// Xóa lesson (hard delete)
        /// </summary>
        public async Task DeleteLessonAsync(int id)
        {
            var lesson = await _context.lessons
                .Include(l => l.LessonDetails) // Include để check cascade
                .Include(l => l.Progresses)
                .FirstOrDefaultAsync(l => l.LessonId == id);

            if (lesson == null)
                throw new KeyNotFoundException($"Lesson với ID {id} không tồn tại");

            // Kiểm tra có Progress không - không cho xóa nếu có học sinh đang học
            if (lesson.Progresses != null && lesson.Progresses.Any())
            {
                throw new InvalidOperationException(
                    $"Không thể xóa lesson vì có {lesson.Progresses.Count} học sinh đang học bài này. " +
                    "Hãy ẩn lesson thay vì xóa (set IsShared = false)."
                );
            }

            // LessonDetails sẽ tự động xóa nếu có OnDelete Cascade trong DB
            // Hoặc xóa thủ công:
            if (lesson.LessonDetails != null && lesson.LessonDetails.Any())
            {
                _context.lessonDetails.RemoveRange(lesson.LessonDetails);
            }

            _context.lessons.Remove(lesson);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Kiểm tra lesson có tồn tại không
        /// </summary>
        public async Task<bool> LessonExistsAsync(int id)
        {
            return await _context.lessons.AnyAsync(l => l.LessonId == id);
        }

        // ================== QUERY METHODS ==================

        /// <summary>
        /// Lấy tất cả lessons của một giáo án, sắp xếp theo Order
        /// </summary>
        public async Task<IEnumerable<Lesson>> GetLessonsByLessonPlanIdAsync(int lessonPlanId)
        {
            return await _context.lessons
                .Where(l => l.LessonPlanId == lessonPlanId)
                .OrderBy(l => l.Order)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Lấy các lessons được chia sẻ công khai
        /// </summary>
        public async Task<IEnumerable<Lesson>> GetSharedLessonsAsync()
        {
            return await _context.lessons
                .Include(l => l.LessonPlan)
                .Where(l => l.IsShared)
                .OrderByDescending(l => l.PublishedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Lấy Order lớn nhất của một giáo án (để auto-increment khi thêm mới)
        /// </summary>
        public async Task<int> GetMaxOrderByLessonPlanIdAsync(int lessonPlanId)
        {
            var maxOrder = await _context.lessons
                .Where(l => l.LessonPlanId == lessonPlanId)
                .MaxAsync(l => (int?)l.Order); // Cast to nullable để tránh exception khi empty

            return maxOrder ?? 0; // Return 0 nếu chưa có lesson nào
        }

        /// <summary>
        /// Kiểm tra Order có bị trùng không (trong cùng 1 LessonPlan)
        /// </summary>
        public async Task<bool> IsOrderExistsAsync(int lessonPlanId, int order, int? excludeLessonId = null)
        {
            var query = _context.lessons
                .Where(l => l.LessonPlanId == lessonPlanId && l.Order == order);

            // Exclude lesson hiện tại khi update
            if (excludeLessonId.HasValue)
                query = query.Where(l => l.LessonId != excludeLessonId.Value);

            return await query.AnyAsync();
        }

        /// <summary>
        /// Đếm số lessons của một giáo án
        /// </summary>
        public async Task<int> CountLessonsByLessonPlanIdAsync(int lessonPlanId)
        {
            return await _context.lessons
                .CountAsync(l => l.LessonPlanId == lessonPlanId);
        }

        /// <summary>
        /// Kiểm tra lesson có Progress nào không (để quyết định có cho xóa không)
        /// </summary>
        public async Task<bool> HasProgressAsync(int lessonId)
        {
            return await _context.progresses
                .AnyAsync(p => p.LessonId == lessonId);
        }

        /// <summary>
        /// Cập nhật nhiều lessons cùng lúc (bulk update)
        /// </summary>
        public async Task UpdateRangeAsync(IEnumerable<Lesson> lessons)
        {
            if (lessons == null || !lessons.Any())
                return;

            // Mark tất cả entities là Modified
            foreach (var lesson in lessons)
            {
                _context.Entry(lesson).State = EntityState.Modified;
            }

            // Gọi SaveChanges một lần duy nhất
            await _context.SaveChangesAsync();
        }
    }
}
