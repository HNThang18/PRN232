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
    public class SubmissionDetailRepository : GenericRepository<SubmissionDetail>, ISubmissionDetailRepository
    {
        private readonly MathLpContext _context;
        public SubmissionDetailRepository(MathLpContext context) : base(context)
        {
        }

        public async Task<int> AddRangeAsync(IEnumerable<SubmissionDetail> details)
        {
            await _context.submissionDetails.AddRangeAsync(details);
            return await _context.SaveChangesAsync();
        }
    }
}
