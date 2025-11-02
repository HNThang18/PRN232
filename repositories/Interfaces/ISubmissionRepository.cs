using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Interfaces
{
    public interface ISubmissionRepository : IGenericRepository<Submission>
    {
        Task<List<Submission>> GetSubmissionsByStudentAndQuizAsync(int studentId, int quizId);
        Task<int> GetSubmissionCountAsync(int studentId, int quizId);
        Task<Submission> GetSubmissionWithDetailsAsync(int submissionId);
    }
}
