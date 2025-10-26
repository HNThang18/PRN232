using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace services.Services
{
    public class LessonPlanService : ILessonPlanService
    {
        private readonly ILessonPlanRepository _lessonPlanRepository;

        public LessonPlanService(ILessonPlanRepository lessonPlanRepository)
        {
            _lessonPlanRepository = lessonPlanRepository;
        }

        public async Task<IEnumerable<LessonPlan>> GetAllLessonPlansAsync(CancellationToken cancellationToken = default)
        {
            // Có thể thêm logic xử lý trước khi gọi repository (nếu cần)
            return await _lessonPlanRepository.GetAllLessonPlansAsync(cancellationToken);
        }

        public async Task<LessonPlan?> GetLessonPlanByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // Kiểm tra ID hợp lệ
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than 0.");
            }

            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(id, cancellationToken);

            // Thêm logic kiểm tra nếu cần
            if (lessonPlan == null)
            {
                throw new KeyNotFoundException($"LessonPlan with ID {id} not found.");
            }

            return lessonPlan;
        }

        public async Task AddLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default)
        {
            // Validation logic trước khi thêm
            if (string.IsNullOrWhiteSpace(lessonPlan.Title))
            {
                throw new ArgumentException("LessonPlan title cannot be empty.");
            }

            await _lessonPlanRepository.AddLessonPlanAsync(lessonPlan, cancellationToken);
        }

        public async Task UpdateLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default)
        {
            // Validation logic trước khi cập nhật
            if (lessonPlan.LessonPlanId <= 0)
            {
                throw new ArgumentException("Invalid LessonPlan ID.");
            }

            await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan, cancellationToken);
        }

        public async Task DeleteLessonPlanAsync(int id, CancellationToken cancellationToken = default)
        {
            // Kiểm tra ID hợp lệ
            if (id <= 0)
            {
                throw new ArgumentException("ID must be greater than 0.");
            }

            await _lessonPlanRepository.DeleteLessonPlanAsync(id, cancellationToken);
        }
    }
}
