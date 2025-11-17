using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Repositories
{
    public class ProgressRepository : GenericRepository<Progress>, IProgressRepository
    {
        private readonly MathLpContext _context;
        public ProgressRepository(MathLpContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Progress> GetByStudentAndLessonAsync(int studentId, int lessonId)
        {
            return await _context.progresses
                .FirstOrDefaultAsync(p => p.StudentId == studentId && p.LessonId == lessonId);
        }
        public async Task<List<Progress>> GetByStudentAndLessonPlanAsync(int studentId, int lessonPlanId)
        {
            
            return await _context.progresses
                .Include(p => p.Lesson)
                .Where(p => p.StudentId == studentId && p.Lesson.LessonPlanId == lessonPlanId)
                .ToListAsync();
        }
        public async Task<List<Progress>> GetByStudentAsync(int studentId)
        {
            return await _context.progresses
                .Where(p => p.StudentId == studentId)
                .Include(p => p.Lesson)
                .ToListAsync();
        }

    }
}
