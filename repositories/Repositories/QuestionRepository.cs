using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
