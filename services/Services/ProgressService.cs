using applications.DTOs.Response.Progress;
using repositories.Interfaces;
using repositories.Models;
using services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace services.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressRepository _progressRepo;
        private readonly ILessonRepository _lessonRepo;
        private readonly ISubmissionRepository _submissionRepo;

        public ProgressService(
            IProgressRepository progressRepo,
            ILessonRepository lessonRepo,
            ISubmissionRepository submissionRepo)
        {
            _progressRepo = progressRepo;
            _lessonRepo = lessonRepo;
            _submissionRepo = submissionRepo;
        }

        // === 1. Mark lesson ===
        public async Task<bool> MarkLessonAsCompleted(int lessonId, int studentId)
        {
            var progress = await _progressRepo.GetByStudentAndLessonAsync(studentId, lessonId);
            if (progress == null)
            {
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
                progress.CompletionStatus = ProgressStatus.Completed;
                progress.AttemptDate = DateTime.UtcNow;
                var result = await _progressRepo.UpdateAsync(progress);
                return result > 0;
            }
        }

        public async Task<bool> MarkLessonAsInProgress(int lessonId, int studentId)
        {
            var progress = await _progressRepo.GetByStudentAndLessonAsync(studentId, lessonId);
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
            return true;
        }

        // === 2. Progress từng lesson ===
        public async Task<List<ProgressResponse>> GetLessonPlanProgressAsync(int lessonPlanId, int studentId)
        {
            var lessons = await _lessonRepo.GetLessonsByLessonPlanIdAsync(lessonPlanId);
            var progresses = await _progressRepo.GetByStudentAndLessonPlanAsync(studentId, lessonPlanId);

            var response = lessons.Select(lesson =>
            {
                var progress = progresses.FirstOrDefault(p => p.LessonId == lesson.LessonId);
                return new ProgressResponse
                {
                    LessonId = lesson.LessonId,
                    CompletionStatus = progress?.CompletionStatus.ToString() ?? ProgressStatus.NotStarted.ToString(),
                    AttemptDate = progress?.AttemptDate
                };
            }).ToList();

            return response;
        }

        // === 3. Overall dashboard ===
        public async Task<StudentProgressResponse> GetOverallProgressAsync(int studentId)
        {
            var submissions = await _submissionRepo.GetSubmissionsByStudentAsync(studentId);
            var progresses = await _progressRepo.GetByStudentAsync(studentId);
            var lessons = await _lessonRepo.GetAllLessonsAsync();

            int quizzesCompleted = submissions.Count(s => s.Status == SubissionStatus.Completed);
            int topicsMastered = progresses.Count(p => p.CompletionStatus == ProgressStatus.Completed);

            // Cast decimal -> double
            double overallScore = submissions.Any() ? submissions.Average(s => (double)s.Score) : 0;

            // Tính % thay đổi tuần trước
            var lastWeek = DateTime.UtcNow.AddDays(-7);
            double thisWeekScore = submissions
                .Where(s => s.SubmittedAt >= lastWeek)
                .Select(s => (double)s.Score)
                .DefaultIfEmpty(0)
                .Average();

            double previousWeekScore = submissions
                .Where(s => s.SubmittedAt < lastWeek && s.SubmittedAt >= lastWeek.AddDays(-7))
                .Select(s => (double)s.Score)
                .DefaultIfEmpty(0)
                .Average();

            double scoreChangePercent = previousWeekScore == 0 ? 0 : ((thisWeekScore - previousWeekScore) / previousWeekScore) * 100;

            return new StudentProgressResponse
            {
                OverallScore = overallScore,
                QuizzesCompleted = quizzesCompleted,
                TopicsMastered = topicsMastered,
                ScoreChangePercent = scoreChangePercent
            };
        }

        // === 4. Score trend theo tuần ===
        public async Task<List<ScoreTrendResponse>> GetScoreTrendAsync(int studentId)
        {
            var submissions = await _submissionRepo.GetSubmissionsByStudentAsync(studentId);

            var trend = submissions
                .GroupBy(s => ISOWeek.GetWeekOfYear(s.SubmittedAt))
                .OrderBy(g => g.Key)
                .Select(g => new ScoreTrendResponse
                {
                    Week = $"Week {g.Key}",
                    Score = g.Average(s => (double)s.Score) // cast decimal -> double
                })
                .ToList();

            return trend;
        }

        // === 5. Topic progress ===
        public async Task<List<TopicProgressResponse>> GetTopicProgressAsync(int studentId)
        {
            var lessons = await _lessonRepo.GetAllLessonsAsync();
            var progresses = await _progressRepo.GetByStudentAsync(studentId);

            var response = lessons.Select(lesson =>
            {
                var progress = progresses.FirstOrDefault(p => p.LessonId == lesson.LessonId);
                bool completed = progress != null && progress.CompletionStatus == ProgressStatus.Completed;

                return new TopicProgressResponse
                {
                    TopicName = lesson.Title,
                    Completed = completed ? 1 : 0,
                    Total = 1
                };
            }).ToList();

            return response;
        }

        // === 6. Areas for improvement ===
        public async Task<List<AreaForImprovementResponse>> GetAreasForImprovementAsync(int studentId)
        {
            var lessons = await _lessonRepo.GetAllLessonsAsync();
            var progresses = await _progressRepo.GetByStudentAsync(studentId);

            var response = lessons.Select(lesson =>
            {
                var progress = progresses.FirstOrDefault(p => p.LessonId == lesson.LessonId);
                double achievedPercent = progress != null && progress.CompletionStatus == ProgressStatus.Completed ? 100 : 0;

                return new AreaForImprovementResponse
                {
                    TopicName = lesson.Title,
                    Description = lesson.Objective ?? "",
                    AchievedPercent = achievedPercent
                };
            })
            .Where(r => r.AchievedPercent < 80) // chỉ lấy topic cần cải thiện
            .ToList();

            return response;
        }
    }
}
