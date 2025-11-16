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
        public SubmissionRepository(MathLpContext context) : base(context)
        {
        }

        public async Task<List<Submission>> GetSubmissionsByStudentAndQuizAsync(int studentId, int quizId)
        {
            // S?a .CountAsync() thành .Where(...) và .ToListAsync()
            return await _context.submissions // (Gi? s? tên DbSet c?a b?n là 'submissions')
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
            return await _context.submissions
                .Include(s => s.Quiz)
                .Include(s => s.SubmissionDetails)
                    .ThenInclude(sd => sd.Question)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId) ?? throw new Exception("Submission not found");
        }

        public async Task<IEnumerable<Submission>> GetSubmissionsByQuizIdAsync(int quizId)
        {
            return await _context.submissions
                .Include(s => s.Student)
                .Include(s => s.SubmissionDetails)
                .Where(s => s.QuizId == quizId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }

        public async Task<int> GetSubmissionCountByQuizIdAsync(int quizId)
        {
            return await _context.submissions
                .Where(s => s.QuizId == quizId)
                .CountAsync();
        }

        public async Task<decimal> GetAverageScoreByQuizIdAsync(int quizId)
        {
            var submissions = await _context.submissions
                .Where(s => s.QuizId == quizId && s.Status == SubissionStatus.Completed)
                .ToListAsync();

            if (!submissions.Any())
                return 0;

            return submissions.Average(s => s.Score);
        }
    }
}
