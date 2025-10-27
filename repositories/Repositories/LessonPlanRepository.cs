using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace repositories.Repositories
{
    public class LessonPlanRepository : GenericRepository<LessonPlan>, ILessonPlanRepository
    {
        public LessonPlanRepository(MathLpContext context) : base(context) { }

        // ================== BASIC CRUD ==================

        /// <summary>
        /// Lấy tất cả lesson plans (không include lessons - cho list view)
        /// </summary>
        public async Task<IEnumerable<LessonPlan>> GetAllLessonPlansAsync(CancellationToken cancellationToken = default)
        {
            return await _context.lessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Level)
                .Where(lp => lp.Status != LessonPlanStatus.Deleted) // Không hiển thị deleted
                .OrderByDescending(lp => lp.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Lấy lesson plan theo ID (không include lessons - cho edit form)
        /// </summary>
        public async Task<LessonPlan?> GetLessonPlanByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.lessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Level)
                .Include(lp => lp.AiRequest)
                .Where(lp => lp.Status != LessonPlanStatus.Deleted)
                .FirstOrDefaultAsync(lp => lp.LessonPlanId == id, cancellationToken);
        }

        /// <summary>
        /// Lấy lesson plan với tất cả lessons (cho detail view)
        /// </summary>
        public async Task<LessonPlan?> GetLessonPlanWithLessonsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.lessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Level)
                .Include(lp => lp.AiRequest)
                .Include(lp => lp.Lessons.OrderBy(l => l.Order))
                    .ThenInclude(l => l.LessonDetails.OrderBy(d => d.Order))
                .Where(lp => lp.Status != LessonPlanStatus.Deleted)
                .FirstOrDefaultAsync(lp => lp.LessonPlanId == id, cancellationToken);
        }

        /// <summary>
        /// Thêm lesson plan mới
        /// </summary>
        public async Task AddLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default)
        {
            if (lessonPlan.CreatedAt == default)
                lessonPlan.CreatedAt = DateTime.UtcNow;

            await _context.lessonPlans.AddAsync(lessonPlan, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Cập nhật lesson plan
        /// </summary>
        public async Task UpdateLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default)
        {
            var existingLessonPlan = await _context.lessonPlans
                .FirstOrDefaultAsync(lp => lp.LessonPlanId == lessonPlan.LessonPlanId && lp.Status != LessonPlanStatus.Deleted, cancellationToken);

            if (existingLessonPlan == null)
                throw new KeyNotFoundException($"LessonPlan with ID {lessonPlan.LessonPlanId} not found.");

            lessonPlan.UpdatedAt = DateTime.UtcNow;

            _context.Entry(existingLessonPlan).CurrentValues.SetValues(lessonPlan);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await LessonPlanExistsAsync(lessonPlan.LessonPlanId))
                    throw new KeyNotFoundException($"LessonPlan with ID {lessonPlan.LessonPlanId} was deleted.");
                throw;
            }
        }

        /// <summary>
        /// Xóa lesson plan (soft delete)
        /// </summary>
        public async Task DeleteLessonPlanAsync(int id, CancellationToken cancellationToken = default)
        {
            var lessonPlan = await _context.lessonPlans
                .FirstOrDefaultAsync(lp => lp.LessonPlanId == id && lp.Status != LessonPlanStatus.Deleted, cancellationToken);

            if (lessonPlan == null)
                throw new KeyNotFoundException($"LessonPlan with ID {id} not found.");

            lessonPlan.Status = LessonPlanStatus.Deleted;
            lessonPlan.UpdatedAt = DateTime.UtcNow;

            _context.lessonPlans.Update(lessonPlan);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Kiểm tra lesson plan có tồn tại không
        /// </summary>
        public async Task<bool> LessonPlanExistsAsync(int id)
        {
            return await _context.lessonPlans
                .AnyAsync(lp => lp.LessonPlanId == id && lp.Status != LessonPlanStatus.Deleted);
        }

        // ================== QUERY METHODS ==================

        /// <summary>
        /// Lấy tất cả lesson plans của một teacher
        /// </summary>
        public async Task<IEnumerable<LessonPlan>> GetLessonPlansByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.lessonPlans
                .Include(lp => lp.Level)
                .Where(lp => lp.TeacherId == teacherId && lp.Status != LessonPlanStatus.Deleted)
                .OrderByDescending(lp => lp.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Lấy tất cả lesson plans của một level
        /// </summary>
        public async Task<IEnumerable<LessonPlan>> GetLessonPlansByLevelIdAsync(int levelId, CancellationToken cancellationToken = default)
        {
            return await _context.lessonPlans
                .Include(lp => lp.Teacher)
                .Where(lp => lp.LevelId == levelId && lp.Status != LessonPlanStatus.Deleted)
                .OrderByDescending(lp => lp.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Lấy lesson plans theo status
        /// </summary>
        public async Task<IEnumerable<LessonPlan>> GetLessonPlansByStatusAsync(LessonPlanStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.lessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Level)
                .Where(lp => lp.Status == status)
                .OrderByDescending(lp => lp.PublishedAt ?? lp.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Lấy lesson plans công khai
        /// </summary>
        public async Task<IEnumerable<LessonPlan>> GetPublicLessonPlansAsync(CancellationToken cancellationToken = default)
        {
            return await _context.lessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Level)
                .Where(lp => lp.IsPublic && lp.Status == LessonPlanStatus.Published)
                .OrderByDescending(lp => lp.PublishedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Tìm kiếm lesson plans theo keyword (title, topic, tags)
        /// </summary>
        public async Task<IEnumerable<LessonPlan>> SearchLessonPlansAsync(string keyword, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllLessonPlansAsync(cancellationToken);

            keyword = keyword.ToLower();

            return await _context.lessonPlans
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Level)
                .Where(lp => lp.Status != LessonPlanStatus.Deleted &&
                            (lp.Title.ToLower().Contains(keyword) ||
                             (lp.Topic != null && lp.Topic.ToLower().Contains(keyword)) ||
                             (lp.Tags != null && lp.Tags.ToLower().Contains(keyword))))
                .OrderByDescending(lp => lp.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Đếm số lessons trong một lesson plan
        /// </summary>
        public async Task<int> CountLessonsByLessonPlanIdAsync(int lessonPlanId, CancellationToken cancellationToken = default)
        {
            return await _context.lessons
                .CountAsync(l => l.LessonPlanId == lessonPlanId, cancellationToken);
        }

        /// <summary>
        /// Bulk update lesson plans
        /// </summary>
        public async Task UpdateRangeAsync(IEnumerable<LessonPlan> lessonPlans, CancellationToken cancellationToken = default)
        {
            if (lessonPlans == null || !lessonPlans.Any())
                return;

            foreach (var lessonPlan in lessonPlans)
            {
                _context.Entry(lessonPlan).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
