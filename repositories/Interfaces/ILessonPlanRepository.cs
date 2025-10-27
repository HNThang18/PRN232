using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using repositories.Models;

namespace repositories.Interfaces
{
    public interface ILessonPlanRepository
    {
        // Basic CRUD
        Task<IEnumerable<LessonPlan>> GetAllLessonPlansAsync(CancellationToken cancellationToken = default);
        Task<LessonPlan?> GetLessonPlanByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<LessonPlan?> GetLessonPlanWithLessonsAsync(int id, CancellationToken cancellationToken = default);
        Task AddLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default);
        Task UpdateLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default);
        Task DeleteLessonPlanAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> LessonPlanExistsAsync(int id);

        // Query methods
        Task<IEnumerable<LessonPlan>> GetLessonPlansByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);
        Task<IEnumerable<LessonPlan>> GetLessonPlansByLevelIdAsync(int levelId, CancellationToken cancellationToken = default);
        Task<IEnumerable<LessonPlan>> GetLessonPlansByStatusAsync(LessonPlanStatus status, CancellationToken cancellationToken = default);
        Task<IEnumerable<LessonPlan>> GetPublicLessonPlansAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<LessonPlan>> SearchLessonPlansAsync(string keyword, CancellationToken cancellationToken = default);
        Task<int> CountLessonsByLessonPlanIdAsync(int lessonPlanId, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(IEnumerable<LessonPlan> lessonPlans, CancellationToken cancellationToken = default);
    }
}
