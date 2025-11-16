using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace repositories.Repositories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(MathLpContext context) : base(context) { }

        public async Task AddQuestionAsync(Question question)
        {
            await _context.questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Question>> GetQuestionsWithCorrectAnswersAsync(List<int> questionIds)
        {
            return await _context.questions
                .Where(q => questionIds.Contains(q.QuestionId))
                .ToListAsync();
        }

        public async Task<List<Question>> GetAllAsync()
        {
            return await _context.questions.ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.questions.FindAsync(id);
        }

        public async Task<int> CreateAsync(Question question)
        {
            await _context.questions.AddAsync(question);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Question question)
        {
            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(question);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveAsync(Question question)
        {
            _context.questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Question>> GetByQuizIdAsync(int quizId)
        {
            return await _context.questions.Where(q => q.QuizId == quizId).ToListAsync();
        }
    }
}
