using Microsoft.EntityFrameworkCore;
using repositories.Basic;
using repositories.Dbcontext;
using repositories.Interfaces;
using repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace repositories.Repositories
{
    public class QuestionBankRepository : GenericRepository<QuestionBank>, IQuestionBankRepository
    {
        public QuestionBankRepository(MathLpContext context) : base(context) { }

        public async Task<IEnumerable<QuestionBank>> GetAllQuestionBanksAsync()
        {
            return await _context.questionBanks.Include(qb => qb.Teacher).Include(qb => qb.Level).ToListAsync();
        }

        public async Task<QuestionBank?> GetQuestionBankByIdAsync(int id)
        {
            return await _context.questionBanks.Include(qb => qb.Teacher).Include(qb => qb.Level).FirstOrDefaultAsync(qb => qb.QuestionBankId == id);
        }

        public async Task AddQuestionBankAsync(QuestionBank questionBank)
        {
            await _context.questionBanks.AddAsync(questionBank);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuestionBankAsync(QuestionBank questionBank)
        {
            _context.Entry(questionBank).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionBankAsync(int id)
        {
            var entity = await _context.questionBanks.FindAsync(id);
            if (entity != null)
            {
                _context.questionBanks.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
