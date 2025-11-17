using applications.DTOs.Response.Progress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services.Interfaces
{
    public interface IProgressService
    {
       
        Task<bool> MarkLessonAsCompleted(int lessonId, int studentId);

        Task<bool> MarkLessonAsInProgress(int lessonId, int studentId);

        Task<List<ProgressResponse>> GetLessonPlanProgressAsync(int lessonPlanId, int studentId);
        Task<StudentProgressResponse> GetOverallProgressAsync(int studentId);
        Task<List<ScoreTrendResponse>> GetScoreTrendAsync(int studentId);
        Task<List<TopicProgressResponse>> GetTopicProgressAsync(int studentId);
        Task<List<AreaForImprovementResponse>> GetAreasForImprovementAsync(int studentId);

    }
}
