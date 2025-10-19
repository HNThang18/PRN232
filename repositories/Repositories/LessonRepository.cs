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
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(MathLpContext context) : base(context) {}

        public async Task AddLessonAsync(Lesson lesson)
        {
            await _context.lessons.AddAsync(lesson);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLessonAsync(int id)
        {
            var entity = await _context.lessons.FindAsync(id);
            if (entity != null)
            {
                _context.lessons.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Lesson>> GetAllLessonsAsync()
        {
            return await _context.lessons.ToListAsync();
        }

        public async Task<Lesson?> GetLessonByIdAsync(int id)
        {
          return await _context.lessons.FindAsync(id);
        }

        public async Task UpdateLessonAsync(Lesson lesson)
        {
            _context.Entry(lesson).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
