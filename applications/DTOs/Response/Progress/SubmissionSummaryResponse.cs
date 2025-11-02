using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Response.Progress
{
    public class SubmissionSummaryResponse
    {
        public int SubmissionId { get; set; }
        public decimal Score { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int AttemptNumber { get; set; }
    }
}
