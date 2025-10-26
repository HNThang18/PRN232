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

        public async Task<IEnumerable<LessonPlan>> GetAllLessonPlansAsync(CancellationToken cancellationToken = default)
        {
            return await _context.lessonPlans
                .AsNoTracking()
                .Include(lp => lp.Teacher)
                .Include(lp => lp.Level)
                .Include(lp => lp.AiRequest)
                .Include(lp => lp.Lessons)
                    .ThenInclude(l => l.LessonDetails)
                .ToListAsync(cancellationToken);
        }


        public async Task<LessonPlan?> GetLessonPlanByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.lessonPlans
                .AsNoTracking()
                .Include(lp => lp.Level)
                .Include(lp => lp.Teacher)
                .Include(lp => lp.AiRequest)
                .Include(lp => lp.Lessons)
                    .ThenInclude(l => l.LessonDetails)
                .FirstOrDefaultAsync(lp => lp.LessonPlanId == id, cancellationToken);
        }

        public async Task AddLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default)
        {
            await _context.lessonPlans.AddAsync(lessonPlan, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default)
        {
            var existingLessonPlan = await _context.lessonPlans
                .FindAsync(new object[] { lessonPlan.LessonPlanId }, cancellationToken);
            if (existingLessonPlan == null)
            {
                throw new KeyNotFoundException($"LessonPlan with ID {lessonPlan.LessonPlanId} not found.");
            }

            // Chỉ sao chép các thuộc tính vô hướng để tránh ghi đè vô tình các bộ sưu tập/trạng thái điều hướng
            _context.Entry(existingLessonPlan).CurrentValues.SetValues(lessonPlan);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteLessonPlanAsync(int id, CancellationToken cancellationToken = default)
        {
            var lessonPlan = await _context.lessonPlans.FindAsync(new object[] { id }, cancellationToken);
            if (lessonPlan == null)
            {
                throw new KeyNotFoundException($"LessonPlan with ID {id} not found.");
            }
            lessonPlan.Status = LessonPlanStatus.Deleted;
            _context.lessonPlans.Update(lessonPlan);
            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
