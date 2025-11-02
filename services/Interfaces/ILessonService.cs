using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using repositories.Models;

namespace services.Interfaces
{
    public interface ILessonService
    {
        // Basic CRUD
        Task<IEnumerable<Lesson>> GetAllLessonsAsync();
        Task<Lesson?> GetLessonByIdAsync(int id);
        Task<Lesson?> GetLessonWithDetailsAsync(int id);
        Task<Lesson> AddLessonAsync(Lesson lesson);
        Task UpdateLessonAsync(Lesson lesson);
        Task DeleteLessonAsync(int id);

        // Business methods
        Task<IEnumerable<Lesson>> GetLessonsByLessonPlanIdAsync(int lessonPlanId);
        Task<IEnumerable<Lesson>> GetSharedLessonsAsync();
        Task PublishLessonAsync(int lessonId); // ⭐ Publish lesson
        Task UnpublishLessonAsync(int lessonId); // ⭐ Unpublish lesson
        Task ReorderLessonsAsync(int lessonPlanId, Dictionary<int, int> newOrders); // ⭐ Reorder
        Task<bool> CanDeleteLessonAsync(int lessonId); // ⭐ Check có thể xóa không
    }
}