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

        public async Task<IEnumerable<Quiz>> GetByTeacherIdAsync(int teacherId, int page, int limit)
        {
            return await _context.quizzes
                .Include(q => q.Teacher)
                .Include(q => q.Level)
                .Include(q => q.Questions)
                .Include(q => q.Submissions)
                .Where(q => q.TeacherId == teacherId)
                .OrderByDescending(q => q.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetCountByTeacherIdAsync(int teacherId, QuizStatus? status)
        {
            var query = _context.quizzes.Where(q => q.TeacherId == teacherId);
            
            if (status.HasValue)
            {
                query = query.Where(q => q.Status == status.Value);
            }
            
            return await query.CountAsync();
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

        public async Task<IEnumerable<Quiz>> GetByStatusAsync(QuizStatus status, int page, int limit)
        {
            return await _context.quizzes
                .Include(q => q.Teacher)
                .Include(q => q.Level)
                .Include(q => q.Questions)
                .Include(q => q.Submissions)
                .Where(q => q.Status == status)
                .OrderByDescending(q => q.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetCountByStatusAsync(QuizStatus? status)
        {
            if (status.HasValue)
            {
                return await _context.quizzes.Where(q => q.Status == status.Value).CountAsync();
            }
            return await _context.quizzes.CountAsync();
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

        public async Task<Quiz> GetQuizWithDetailsAsync(int quizId)
        {
            return await _context.quizzes
                .Include(q => q.Questions)  
                    .ThenInclude(ques => ques.Answers)
                .FirstOrDefaultAsync(q => q.QuizId == quizId) ?? throw new Exception("Quiz not found");
        }

        public async Task<IEnumerable<Quiz>> SearchQuizzesAsync(string? keyword, int? levelId, QuizStatus? status, int? teacherId, int page, int limit)
        {
            var query = _context.quizzes
                .Include(q => q.Teacher)
                .Include(q => q.Level)
                .Include(q => q.Questions)
                .Include(q => q.Submissions)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(q => q.Title.Contains(keyword));
            }

            if (levelId.HasValue)
            {
                query = query.Where(q => q.LevelId == levelId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(q => q.Status == status.Value);
            }

            if (teacherId.HasValue)
            {
                query = query.Where(q => q.TeacherId == teacherId.Value);
            }

            return await query
                .OrderByDescending(q => q.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetSearchCountAsync(string? keyword, int? levelId, QuizStatus? status, int? teacherId)
        {
            var query = _context.quizzes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(q => q.Title.Contains(keyword));
            }

            if (levelId.HasValue)
            {
                query = query.Where(q => q.LevelId == levelId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(q => q.Status == status.Value);
            }

            if (teacherId.HasValue)
            {
                query = query.Where(q => q.TeacherId == teacherId.Value);
            }

            return await query.CountAsync();
        }
    }
}
