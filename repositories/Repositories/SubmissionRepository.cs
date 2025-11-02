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
    public class SubmissionRepository : GenericRepository<Submission>, ISubmissionRepository
    {
        private readonly MathLpContext _context;
        public SubmissionRepository(MathLpContext context) : base(context)
        {
        }

        public async Task<List<Submission>> GetSubmissionsByStudentAndQuizAsync(int studentId, int quizId)
        {
            // Sửa .CountAsync() thành .Where(...) và .ToListAsync()
            return await _context.submissions // (Giả sử tên DbSet của bạn là 'submissions')
                .Where(s => s.StudentId == studentId && s.QuizId == quizId)
                .ToListAsync();
        }
        public async Task<int> GetSubmissionCountAsync(int studentId, int quizId) { 
            return await _context.submissions
                .Where(s => s.StudentId == studentId && s.QuizId == quizId)
                .CountAsync();
        }

        public async Task<Submission> GetSubmissionWithDetailsAsync(int submissionId)
        {
            return await _context.submissions // (hoặc _context.Submissions)
                .Include(s => s.Quiz) // Lấy thông tin Quiz (để lấy QuizTitle)
                .Include(s => s.SubmissionDetails) // Lấy danh sách chi tiết
                    .ThenInclude(sd => sd.Question) // Với MỖI chi tiết, lấy Question tương ứng
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);
        }
    }
}
