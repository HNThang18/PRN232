using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Interfaces
{
    public interface IProgressRepository : IGenericRepository<Progress>
    {
        Task<Progress> GetByStudentAndLessonAsync(int studentId, int lessonId);
        Task<List<Progress>> GetByStudentAndLessonPlanAsync(int studentId, int lessonPlanId);
    }
}
