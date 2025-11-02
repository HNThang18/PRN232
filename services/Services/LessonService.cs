using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;

namespace services.Services
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ILessonPlanRepository _lessonPlanRepository; // ⭐ Cần thêm

        public LessonService(
            ILessonRepository lessonRepository,
            ILessonPlanRepository lessonPlanRepository) // ⭐ Inject thêm
        {
            _lessonRepository = lessonRepository;
            _lessonPlanRepository = lessonPlanRepository;
        }

        // ================== BASIC CRUD ==================

        public async Task<IEnumerable<Lesson>> GetAllLessonsAsync()
        {
            return await _lessonRepository.GetAllLessonsAsync();
        }

        public async Task<Lesson?> GetLessonByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID phải lớn hơn 0", nameof(id));

            return await _lessonRepository.GetLessonByIdAsync(id);
        }

        public async Task<Lesson?> GetLessonWithDetailsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID phải lớn hơn 0", nameof(id));

            var lesson = await _lessonRepository.GetLessonWithDetailsAsync(id);
            if (lesson == null)
                throw new KeyNotFoundException($"Không tìm thấy lesson với ID {id}");

            return lesson;
        }

        public async Task<Lesson> AddLessonAsync(Lesson lesson)
        {
            // ===== VALIDATION =====

            // 1. Validate input
            if (lesson == null)
                throw new ArgumentNullException(nameof(lesson));

            if (string.IsNullOrWhiteSpace(lesson.Title))
                throw new ArgumentException("Tiêu đề bài học không được để trống");

            if (lesson.LessonPlanId <= 0)
                throw new ArgumentException("LessonPlanId không hợp lệ");

            // 2. Check LessonPlan có tồn tại không
            var lessonPlanExists = await _lessonPlanRepository.LessonPlanExistsAsync(lesson.LessonPlanId);
            if (!lessonPlanExists)
                throw new KeyNotFoundException($"Không tìm thấy giáo án với ID {lesson.LessonPlanId}");

            // 3. Auto-set Order nếu không được cung cấp
            if (lesson.Order <= 0)
            {
                var maxOrder = await _lessonRepository.GetMaxOrderByLessonPlanIdAsync(lesson.LessonPlanId);
                lesson.Order = maxOrder + 1;
            }
            else
            {
                // Kiểm tra Order có bị trùng không
                var orderExists = await _lessonRepository.IsOrderExistsAsync(
                    lesson.LessonPlanId,
                    lesson.Order
                );
                if (orderExists)
                {
                    throw new InvalidOperationException(
                        $"Order {lesson.Order} đã tồn tại trong giáo án này. " +
                        "Vui lòng chọn Order khác hoặc để trống để tự động gán."
                    );
                }
            }

            // 4. Set metadata
            lesson.CreatedAt = DateTime.UtcNow;
            lesson.UpdatedAt = null;

            // 5. Set PublishedDate nếu IsShared = true
            if (lesson.IsShared && lesson.PublishedDate == null)
            {
                lesson.PublishedDate = DateTime.UtcNow;
            }

            // 6. Add to database
            await _lessonRepository.AddLessonAsync(lesson);

            return lesson;
        }

        public async Task UpdateLessonAsync(Lesson lesson)
        {
            // ===== VALIDATION =====

            // 1. Validate input
            if (lesson == null)
                throw new ArgumentNullException(nameof(lesson));

            if (lesson.LessonId <= 0)
                throw new ArgumentException("LessonId không hợp lệ");

            if (string.IsNullOrWhiteSpace(lesson.Title))
                throw new ArgumentException("Tiêu đề bài học không được để trống");

            // 2. Check lesson có tồn tại không
            var exists = await _lessonRepository.LessonExistsAsync(lesson.LessonId);
            if (!exists)
                throw new KeyNotFoundException($"Không tìm thấy lesson với ID {lesson.LessonId}");

            // 3. Check LessonPlan có tồn tại không (nếu thay đổi)
            var lessonPlanExists = await _lessonPlanRepository.LessonPlanExistsAsync(lesson.LessonPlanId);
            if (!lessonPlanExists)
                throw new KeyNotFoundException($"Không tìm thấy giáo án với ID {lesson.LessonPlanId}");

            // 4. Validate Order (nếu thay đổi)
            if (lesson.Order <= 0)
                throw new ArgumentException("Order phải lớn hơn 0");

            var orderExists = await _lessonRepository.IsOrderExistsAsync(
                lesson.LessonPlanId,
                lesson.Order,
                lesson.LessonId // Exclude chính nó
            );
            if (orderExists)
            {
                throw new InvalidOperationException(
                    $"Order {lesson.Order} đã được sử dụng bởi lesson khác trong giáo án này"
                );
            }

            // 5. Update PublishedDate nếu IsShared thay đổi
            var existingLesson = await _lessonRepository.GetLessonByIdAsync(lesson.LessonId);
            if (existingLesson != null)
            {
                // Nếu từ không share → share
                if (!existingLesson.IsShared && lesson.IsShared)
                {
                    lesson.PublishedDate = DateTime.UtcNow;
                }
                // Nếu từ share → không share
                else if (existingLesson.IsShared && !lesson.IsShared)
                {
                    lesson.PublishedDate = null;
                }
            }

            // 6. Set UpdatedAt
            lesson.UpdatedAt = DateTime.UtcNow;

            // 7. Update database
            await _lessonRepository.UpdateLessonAsync(lesson);
        }

        public async Task DeleteLessonAsync(int id)
        {
            // ===== VALIDATION =====

            if (id <= 0)
                throw new ArgumentException("ID phải lớn hơn 0", nameof(id));

            // Check có thể xóa không
            var canDelete = await CanDeleteLessonAsync(id);
            if (!canDelete)
            {
                throw new InvalidOperationException(
                    "Không thể xóa lesson này vì có học sinh đang học. " +
                    "Hãy ẩn lesson (set IsShared = false) thay vì xóa."
                );
            }

            // Delete
            await _lessonRepository.DeleteLessonAsync(id);
        }

        // ================== BUSINESS METHODS ==================

        public async Task<IEnumerable<Lesson>> GetLessonsByLessonPlanIdAsync(int lessonPlanId)
        {
            if (lessonPlanId <= 0)
                throw new ArgumentException("LessonPlanId phải lớn hơn 0", nameof(lessonPlanId));

            return await _lessonRepository.GetLessonsByLessonPlanIdAsync(lessonPlanId);
        }

        public async Task<IEnumerable<Lesson>> GetSharedLessonsAsync()
        {
            return await _lessonRepository.GetSharedLessonsAsync();
        }

        /// <summary>
        /// Publish lesson (share publicly)
        /// </summary>
        public async Task PublishLessonAsync(int lessonId)
        {
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null)
                throw new KeyNotFoundException($"Không tìm thấy lesson với ID {lessonId}");

            if (lesson.IsShared)
                throw new InvalidOperationException("Lesson này đã được publish rồi");

            // Business rule: Phải có content mới được publish
            if (string.IsNullOrWhiteSpace(lesson.Content))
                throw new InvalidOperationException("Lesson phải có nội dung trước khi publish");

            lesson.IsShared = true;
            lesson.PublishedDate = DateTime.UtcNow;
            lesson.UpdatedAt = DateTime.UtcNow;

            await _lessonRepository.UpdateLessonAsync(lesson);
        }

        /// <summary>
        /// Unpublish lesson (make private)
        /// </summary>
        public async Task UnpublishLessonAsync(int lessonId)
        {
            var lesson = await _lessonRepository.GetLessonByIdAsync(lessonId);
            if (lesson == null)
                throw new KeyNotFoundException($"Không tìm thấy lesson với ID {lessonId}");

            if (!lesson.IsShared)
                throw new InvalidOperationException("Lesson này chưa được publish");

            lesson.IsShared = false;
            lesson.PublishedDate = null;
            lesson.UpdatedAt = DateTime.UtcNow;

            await _lessonRepository.UpdateLessonAsync(lesson);
        }

        /// <summary>
        /// Reorder lessons trong một giáo án
        /// </summary>
        /// <param name="lessonPlanId">ID giáo án</param>
        /// <param name="newOrders">Dictionary: Key=LessonId, Value=NewOrder</param>
        public async Task ReorderLessonsAsync(int lessonPlanId, Dictionary<int, int> newOrders)
        {
            if (newOrders == null || !newOrders.Any())
                throw new ArgumentException("NewOrders không được rỗng");

            // Validate tất cả lessons thuộc giáo án này
            var lessons = await _lessonRepository.GetLessonsByLessonPlanIdAsync(lessonPlanId);
            var lessonDict = lessons.ToDictionary(l => l.LessonId);

            // List để lưu các lessons cần update
            var lessonsToUpdate = new List<Lesson>();

            foreach (var kvp in newOrders)
            {
                var lessonId = kvp.Key;
                var newOrder = kvp.Value;

                // Validation
                if (!lessonDict.ContainsKey(lessonId))
                {
                    throw new ArgumentException(
                        $"Lesson {lessonId} không thuộc giáo án {lessonPlanId}"
                    );
                }

                if (newOrder <= 0)
                    throw new ArgumentException($"Order phải > 0, nhưng lesson {lessonId} có order = {newOrder}");

                // Update trong memory
                var lesson = lessonDict[lessonId];
                lesson.Order = newOrder;
                lesson.UpdatedAt = DateTime.UtcNow;

                lessonsToUpdate.Add(lesson);
            }

            // ⭐ Gọi UpdateRange một lần duy nhất thay vì N lần UpdateAsync
            await _lessonRepository.UpdateRangeAsync(lessonsToUpdate);
        }

        /// <summary>
        /// Kiểm tra có thể xóa lesson không
        /// </summary>
        public async Task<bool> CanDeleteLessonAsync(int lessonId)
        {
            // Không cho xóa nếu có học sinh đang học
            var hasProgress = await _lessonRepository.HasProgressAsync(lessonId);
            return !hasProgress;
        }
    }
}