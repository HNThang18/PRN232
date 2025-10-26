using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using repositories.Models;

namespace services.Interfaces
{
    public interface ILessonPlanService
    {
        Task<IEnumerable<LessonPlan>> GetAllLessonPlansAsync(CancellationToken cancellationToken = default);
        Task<LessonPlan?> GetLessonPlanByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default);
        Task UpdateLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default);
        Task DeleteLessonPlanAsync(int id, CancellationToken cancellationToken = default);
    }
}
