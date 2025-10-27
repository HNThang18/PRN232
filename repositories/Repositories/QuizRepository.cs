using Microsoft.EntityFrameworkCore;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;

namespace repositories.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly MathLpContext _context;

        public QuizRepository(MathLpContext context)
        {
            _context = context;
        }

        public async Task<Quiz?> GetByIdAsync(int quizId)
        {
            return await _context.quizzes
                .Include(q => q.Teacher)
                .Include(q => q.Level)
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(q => q.Submissions)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);
        }

        public async Task<IEnumerable<Quiz>> GetAllAsync()
        {
            return await _context.quizzes
                .Include(q => q.Teacher)
                .Include(q => q.Level)
                .Include(q => q.Questions)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetByTeacherIdAsync(int teacherId)
        {
            return await _context.quizzes
                .Include(q => q.Teacher)
                .Include(q => q.Level)
                .Include(q => q.Questions)
                .Where(q => q.TeacherId == teacherId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetByLevelIdAsync(int levelId)
        {
            return await _context.quizzes
                .Include(q => q.Teacher)
                .Include(q => q.Level)
                .Include(q => q.Questions)
                .Where(q => q.LevelId == levelId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetByStatusAsync(QuizStatus status)
        {
            return await _context.quizzes
                .Include(q => q.Teacher)
                .Include(q => q.Level)
                .Include(q => q.Questions)
                .Where(q => q.Status == status)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetAiGeneratedQuizzesAsync()
        {
            return await _context.quizzes
                .Include(q => q.Teacher)
                .Include(q => q.Level)
                .Include(q => q.Questions)
                .Where(q => q.IsAIGenerated)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<Quiz> AddAsync(Quiz quiz)
        {
            await _context.quizzes.AddAsync(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task UpdateAsync(Quiz quiz)
        {
            _context.quizzes.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int quizId)
        {
            var quiz = await _context.quizzes.FindAsync(quizId);
            if (quiz != null)
            {
                _context.quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int quizId)
        {
            return await _context.quizzes.AnyAsync(q => q.QuizId == quizId);
        }
    }
}
