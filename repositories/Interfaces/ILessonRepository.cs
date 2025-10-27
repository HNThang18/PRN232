using repositories.Models;

namespace repositories.Interfaces
{
    public interface ILessonRepository
    {
        // Basic CRUD
        Task<IEnumerable<Lesson>> GetAllLessonsAsync();
        Task<Lesson?> GetLessonByIdAsync(int id);
        Task<Lesson?> GetLessonWithDetailsAsync(int id); // ⭐ Include LessonDetails
        Task AddLessonAsync(Lesson lesson);
        Task UpdateLessonAsync(Lesson lesson);
        Task DeleteLessonAsync(int id);
        Task<bool> LessonExistsAsync(int id); // ⭐ Check exists

        // Query methods
        Task<IEnumerable<Lesson>> GetLessonsByLessonPlanIdAsync(int lessonPlanId); // ⭐ Query theo giáo án
        Task<IEnumerable<Lesson>> GetSharedLessonsAsync(); // ⭐ Lessons công khai
        Task<int> GetMaxOrderByLessonPlanIdAsync(int lessonPlanId); // ⭐ Lấy Order lớn nhất
        Task<bool> IsOrderExistsAsync(int lessonPlanId, int order, int? excludeLessonId = null); // ⭐ Check trùng order
        Task<int> CountLessonsByLessonPlanIdAsync(int lessonPlanId); // ⭐ Đếm số lessons
        Task<bool> HasProgressAsync(int lessonId); // ⭐ Check có Progress không
        Task UpdateRangeAsync(IEnumerable<Lesson> lessons); // ⭐ Bulk update
    }
}