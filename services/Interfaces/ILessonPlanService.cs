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
        // Basic CRUD
        Task<IEnumerable<LessonPlan>> GetAllLessonPlansAsync(CancellationToken cancellationToken = default);
        Task<LessonPlan?> GetLessonPlanByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<LessonPlan?> GetLessonPlanWithLessonsAsync(int id, CancellationToken cancellationToken = default);
        Task<LessonPlan> AddLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default);
        Task UpdateLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default);
        Task DeleteLessonPlanAsync(int id, CancellationToken cancellationToken = default);

        // Query methods
        Task<IEnumerable<LessonPlan>> GetLessonPlansByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);
        Task<IEnumerable<LessonPlan>> GetLessonPlansByLevelIdAsync(int levelId, CancellationToken cancellationToken = default);
        Task<IEnumerable<LessonPlan>> GetPublicLessonPlansAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<LessonPlan>> SearchLessonPlansAsync(string keyword, CancellationToken cancellationToken = default);

        // Business methods
        Task PublishLessonPlanAsync(int lessonPlanId, CancellationToken cancellationToken = default);
        Task UnpublishLessonPlanAsync(int lessonPlanId, CancellationToken cancellationToken = default);
        Task DuplicateLessonPlanAsync(int lessonPlanId, int newTeacherId, CancellationToken cancellationToken = default);
        Task<bool> CanDeleteLessonPlanAsync(int lessonPlanId, CancellationToken cancellationToken = default);
    }
}
