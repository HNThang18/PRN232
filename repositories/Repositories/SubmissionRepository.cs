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
            _context = context;

        }

        public async Task<List<Submission>> GetSubmissionsByStudentAndQuizAsync(int studentId, int quizId)
        {
           
            return await _context.submissions
                .Where(s => s.StudentId == studentId && s.QuizId == quizId)
                .OrderByDescending(s => s.SubmittedAt)
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
                        .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);
        }

        public async Task<List<Submission>> GetSubmissionsByStudentAsync(int studentId)
        {
            return await _context.submissions
                .Where(s => s.StudentId == studentId)
                .Include(s => s.Quiz)
                .ToListAsync();
        }

    }
}
