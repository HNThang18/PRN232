using applications.DTOs.Response.Progress;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressRepository _progressRepo;

        private readonly ILessonRepository _lessonRepo;

        public ProgressService(IProgressRepository progressRepo, ILessonRepository lessonRepo)
        {
            _progressRepo = progressRepo;
            _lessonRepo = lessonRepo;
        }
        public async Task<bool> MarkLessonAsCompleted(int lessonId, int studentId)
        {
            // Logic "Tìm-hoặc-Tạo"
            var progress = await _progressRepo.GetByStudentAndLessonAsync(studentId, lessonId);

            if (progress == null)
            {
                // Nếu chưa có -> Tạo mới
                var newProgress = new Progress
                {
                    StudentId = studentId,
                    LessonId = lessonId,
                    CompletionStatus = ProgressStatus.Completed,
                    AttemptDate = DateTime.UtcNow,
                    IsActive = true
                };
                var result = await _progressRepo.CreateAsync(newProgress);
                return result > 0;
            }
            else
            {
                // Nếu đã có -> Cập nhật
                progress.CompletionStatus = ProgressStatus.Completed;
                progress.AttemptDate = DateTime.UtcNow;
                var result = await _progressRepo.UpdateAsync(progress);
                return result > 0;
            }
        }

        public async Task<bool> MarkLessonAsInProgress(int lessonId, int studentId)
        {
            var progress = await _progressRepo.GetByStudentAndLessonAsync(studentId, lessonId);

            // Chỉ tạo mới nếu chưa tồn tại. Nếu đã "Completed" thì không ghi đè.
            if (progress == null)
            {
                var newProgress = new Progress
                {
                    StudentId = studentId,
                    LessonId = lessonId,
                    CompletionStatus = ProgressStatus.InProgress,
                    AttemptDate = DateTime.UtcNow,
                    IsActive = true
                };
                var result = await _progressRepo.CreateAsync(newProgress);
                return result > 0;
            }
            return true; // Đã tồn tại, xem như thành công
        }

        public async Task<List<ProgressResponse>> GetLessonPlanProgressAsync(int lessonPlanId, int studentId)
        {
            // 1. Lấy tất cả bài học trong LessonPlan
            // (Giả sử ILessonRepository có hàm này)
            var lessons = await _lessonRepo.GetLessonsByLessonPlanIdAsync(lessonPlanId);

            // 2. Lấy tất cả tiến độ của học sinh
            // (Bạn có thể thêm hàm này vào IProgressRepository)
            var progresses = await _progressRepo.GetByStudentAndLessonPlanAsync(studentId, lessonPlanId);

            // 3. Dùng LINQ để map ra DTO
            var response = lessons.Select(lesson =>
            {
                var progress = progresses.FirstOrDefault(p => p.LessonId == lesson.LessonId);
                return new ProgressResponse
                {
                    LessonId = lesson.LessonId,
                    // Nếu không có progress -> "NotStarted"
                    CompletionStatus = progress?.CompletionStatus.ToString() ?? ProgressStatus.NotStarted.ToString(),
                    AttemptDate = progress?.AttemptDate
                };
            }).ToList();

            return response;
        }
    }
}