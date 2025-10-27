using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace services.Services
{
    public class LessonPlanService : ILessonPlanService
    {
        private readonly ILessonPlanRepository _lessonPlanRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILessonRepository _lessonRepository;

        public LessonPlanService(
            ILessonPlanRepository lessonPlanRepository,
            IUserRepository userRepository,
            ILessonRepository lessonRepository)
        {
            _lessonPlanRepository = lessonPlanRepository;
            _userRepository = userRepository;
            _lessonRepository = lessonRepository;
        }

        // ================== BASIC CRUD ==================

        public async Task<IEnumerable<LessonPlan>> GetAllLessonPlansAsync(CancellationToken cancellationToken = default)
        {
            return await _lessonPlanRepository.GetAllLessonPlansAsync(cancellationToken);
        }

        public async Task<LessonPlan?> GetLessonPlanByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than 0.");

            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(id, cancellationToken);

            if (lessonPlan == null)
                throw new KeyNotFoundException($"LessonPlan with ID {id} not found.");

            return lessonPlan;
        }

        public async Task<LessonPlan?> GetLessonPlanWithLessonsAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than 0.");

            var lessonPlan = await _lessonPlanRepository.GetLessonPlanWithLessonsAsync(id, cancellationToken);

            if (lessonPlan == null)
                throw new KeyNotFoundException($"LessonPlan with ID {id} not found.");

            return lessonPlan;
        }

        public async Task<LessonPlan> AddLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default)
        {
            // ===== VALIDATION =====

            if (lessonPlan == null)
                throw new ArgumentNullException(nameof(lessonPlan));

            if (string.IsNullOrWhiteSpace(lessonPlan.Title))
                throw new ArgumentException("LessonPlan title cannot be empty.");

            if (lessonPlan.TeacherId <= 0)
                throw new ArgumentException("Invalid TeacherId.");

            if (lessonPlan.LevelId <= 0)
                throw new ArgumentException("Invalid LevelId.");

            // TODO: Check teacher exists when IUserRepository has UserExistsAsync method
            // var teacherExists = await _userRepository.UserExistsAsync(lessonPlan.TeacherId);
            // if (!teacherExists)
            //     throw new KeyNotFoundException($"Teacher with ID {lessonPlan.TeacherId} not found.");

            // Set default values
            lessonPlan.CreatedAt = DateTime.UtcNow;
            lessonPlan.Status = LessonPlanStatus.Draft;
            lessonPlan.Version = 1;
            lessonPlan.IsPublic = false;

            await _lessonPlanRepository.AddLessonPlanAsync(lessonPlan, cancellationToken);

            return lessonPlan;
        }

        public async Task UpdateLessonPlanAsync(LessonPlan lessonPlan, CancellationToken cancellationToken = default)
        {
            // ===== VALIDATION =====

            if (lessonPlan == null)
                throw new ArgumentNullException(nameof(lessonPlan));

            if (lessonPlan.LessonPlanId <= 0)
                throw new ArgumentException("Invalid LessonPlan ID.");

            if (string.IsNullOrWhiteSpace(lessonPlan.Title))
                throw new ArgumentException("LessonPlan title cannot be empty.");

            // Check exists
            var exists = await _lessonPlanRepository.LessonPlanExistsAsync(lessonPlan.LessonPlanId);
            if (!exists)
                throw new KeyNotFoundException($"LessonPlan with ID {lessonPlan.LessonPlanId} not found.");

            // Get existing to check status changes
            var existing = await _lessonPlanRepository.GetLessonPlanByIdAsync(lessonPlan.LessonPlanId, cancellationToken);

            if (existing != null)
            {
                // If changing from Draft to Published
                if (existing.Status == LessonPlanStatus.Draft && lessonPlan.Status == LessonPlanStatus.Published)
                {
                    // Check if has lessons
                    var lessonCount = await _lessonPlanRepository.CountLessonsByLessonPlanIdAsync(lessonPlan.LessonPlanId, cancellationToken);
                    if (lessonCount == 0)
                        throw new InvalidOperationException("Cannot publish LessonPlan without lessons.");

                    lessonPlan.PublishedAt = DateTime.UtcNow;
                }

                // If unpublishing
                if (existing.Status == LessonPlanStatus.Published && lessonPlan.Status == LessonPlanStatus.Draft)
                {
                    lessonPlan.PublishedAt = null;
                }
            }

            lessonPlan.UpdatedAt = DateTime.UtcNow;

            await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan, cancellationToken);
        }

        public async Task DeleteLessonPlanAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than 0.");

            // Check can delete
            var canDelete = await CanDeleteLessonPlanAsync(id, cancellationToken);
            if (!canDelete)
            {
                throw new InvalidOperationException(
                    "Cannot delete LessonPlan because it has published lessons or student progress. " +
                    "Please archive it instead (set Status = Deleted)."
                );
            }

            await _lessonPlanRepository.DeleteLessonPlanAsync(id, cancellationToken);
        }

        // ================== QUERY METHODS ==================

        public async Task<IEnumerable<LessonPlan>> GetLessonPlansByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            if (teacherId <= 0)
                throw new ArgumentException("TeacherId must be greater than 0.");

            return await _lessonPlanRepository.GetLessonPlansByTeacherIdAsync(teacherId, cancellationToken);
        }

        public async Task<IEnumerable<LessonPlan>> GetLessonPlansByLevelIdAsync(int levelId, CancellationToken cancellationToken = default)
        {
            if (levelId <= 0)
                throw new ArgumentException("LevelId must be greater than 0.");

            return await _lessonPlanRepository.GetLessonPlansByLevelIdAsync(levelId, cancellationToken);
        }

        public async Task<IEnumerable<LessonPlan>> GetPublicLessonPlansAsync(CancellationToken cancellationToken = default)
        {
            return await _lessonPlanRepository.GetPublicLessonPlansAsync(cancellationToken);
        }

        public async Task<IEnumerable<LessonPlan>> SearchLessonPlansAsync(string keyword, CancellationToken cancellationToken = default)
        {
            return await _lessonPlanRepository.SearchLessonPlansAsync(keyword, cancellationToken);
        }

        // ================== BUSINESS METHODS ==================

        /// <summary>
        /// Publish lesson plan (make it available publicly)
        /// </summary>
        public async Task PublishLessonPlanAsync(int lessonPlanId, CancellationToken cancellationToken = default)
        {
            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(lessonPlanId, cancellationToken);

            if (lessonPlan == null)
                throw new KeyNotFoundException($"LessonPlan with ID {lessonPlanId} not found.");

            if (lessonPlan.Status == LessonPlanStatus.Published)
                throw new InvalidOperationException("LessonPlan is already published.");

            // Check if has lessons
            var lessonCount = await _lessonPlanRepository.CountLessonsByLessonPlanIdAsync(lessonPlanId, cancellationToken);
            if (lessonCount == 0)
                throw new InvalidOperationException("Cannot publish LessonPlan without lessons.");

            // Business rule: Must have content
            if (string.IsNullOrWhiteSpace(lessonPlan.Content))
                throw new InvalidOperationException("LessonPlan must have content before publishing.");

            lessonPlan.Status = LessonPlanStatus.Published;
            lessonPlan.PublishedAt = DateTime.UtcNow;
            lessonPlan.UpdatedAt = DateTime.UtcNow;

            await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan, cancellationToken);
        }

        /// <summary>
        /// Unpublish lesson plan (revert to draft)
        /// </summary>
        public async Task UnpublishLessonPlanAsync(int lessonPlanId, CancellationToken cancellationToken = default)
        {
            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(lessonPlanId, cancellationToken);

            if (lessonPlan == null)
                throw new KeyNotFoundException($"LessonPlan with ID {lessonPlanId} not found.");

            if (lessonPlan.Status != LessonPlanStatus.Published)
                throw new InvalidOperationException("LessonPlan is not published.");

            lessonPlan.Status = LessonPlanStatus.Draft;
            lessonPlan.IsPublic = false;
            lessonPlan.PublishedAt = null;
            lessonPlan.UpdatedAt = DateTime.UtcNow;

            await _lessonPlanRepository.UpdateLessonPlanAsync(lessonPlan, cancellationToken);
        }

        /// <summary>
        /// Duplicate lesson plan for another teacher (copy/clone)
        /// </summary>
        public async Task DuplicateLessonPlanAsync(int lessonPlanId, int newTeacherId, CancellationToken cancellationToken = default)
        {
            // Get original with all lessons
            var original = await _lessonPlanRepository.GetLessonPlanWithLessonsAsync(lessonPlanId, cancellationToken);

            if (original == null)
                throw new KeyNotFoundException($"LessonPlan with ID {lessonPlanId} not found.");

            // TODO: Check new teacher exists when IUserRepository has UserExistsAsync
            // var teacherExists = await _userRepository.UserExistsAsync(newTeacherId);
            // if (!teacherExists)
            //     throw new KeyNotFoundException($"Teacher with ID {newTeacherId} not found.");

            // Create duplicate
            var duplicate = new LessonPlan
            {
                TeacherId = newTeacherId,
                LevelId = original.LevelId,
                Grade = original.Grade,
                Title = $"{original.Title} (Copy)",
                Topic = original.Topic,
                LearningObjectives = original.LearningObjectives,
                MathFormulas = original.MathFormulas,
                Duration = original.Duration,
                Content = original.Content,
                Status = LessonPlanStatus.Draft,
                Version = 1,
                IsPublic = false,
                Tags = original.Tags,
                CreatedAt = DateTime.UtcNow
            };

            await _lessonPlanRepository.AddLessonPlanAsync(duplicate, cancellationToken);

            // Duplicate lessons if any
            if (original.Lessons != null && original.Lessons.Any())
            {
                foreach (var lesson in original.Lessons.OrderBy(l => l.Order))
                {
                    var duplicateLesson = new Lesson
                    {
                        LessonPlanId = duplicate.LessonPlanId,
                        Title = lesson.Title,
                        Objective = lesson.Objective,
                        Content = lesson.Content,
                        Example = lesson.Example,
                        ResourceUrl = lesson.ResourceUrl,
                        Order = lesson.Order,
                        IsGeneratedByAI = lesson.IsGeneratedByAI,
                        IsShared = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _lessonRepository.AddLessonAsync(duplicateLesson);
                }
            }
        }

        /// <summary>
        /// Check if lesson plan can be deleted
        /// </summary>
        public async Task<bool> CanDeleteLessonPlanAsync(int lessonPlanId, CancellationToken cancellationToken = default)
        {
            var lessonPlan = await _lessonPlanRepository.GetLessonPlanByIdAsync(lessonPlanId, cancellationToken);

            if (lessonPlan == null)
                return false;

            // Cannot delete if published
            if (lessonPlan.Status == LessonPlanStatus.Published)
                return false;

            // Check if any lessons have student progress
            var lessons = await _lessonRepository.GetLessonsByLessonPlanIdAsync(lessonPlanId);
            foreach (var lesson in lessons)
            {
                var hasProgress = await _lessonRepository.HasProgressAsync(lesson.LessonId);
                if (hasProgress)
                    return false;
            }

            return true;
        }
    }
}
