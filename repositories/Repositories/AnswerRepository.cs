using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace repositories.Repositories
{
    public class AnswerRepository : GenericRepository<Answer>, IAnswerRepository
    {
        private readonly MathLpContext _context;
        public AnswerRepository(MathLpContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Answer answer)
        {
            await _context.answers.AddAsync(answer);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Answer>> GetAllAsync()
        {
            return await _context.answers.ToListAsync();
        }

        public async Task<Answer?> GetByIdAsync(int id)
        {
            return await _context.answers.FindAsync(id);
        }

        public async Task<int> UpdateAsync(Answer answer)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(answer);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Answer answer)
        {
            _context.answers.Remove(answer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Answer>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.answers.Where(a => a.QuestionId == questionId).ToListAsync();
        }
    }
}