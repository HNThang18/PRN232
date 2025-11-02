using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Interfaces
{
    public interface ISubmissionDetailRepository : IGenericRepository<SubmissionDetail>
    {
        Task<int> AddRangeAsync(IEnumerable<SubmissionDetail> details);
    }
}
