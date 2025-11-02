using applications.DTOs.Request.Progress;
using applications.DTOs.Response.Progress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services.Interfaces
{
    public interface ISubmissionService
    {
        // Dùng cho API: POST /api/submissions/start/{quizId}
        Task<QuizStartResponse> StartQuizAsync(int quizId, int studentId);

        // Dùng cho API: POST /api/submissions/{submissionId}/submit
        Task<SubmissionResultResponse> SubmitQuizAsync(int submissionId, SubmitQuizRequest request, int studentId);

        // Dùng cho API: GET /api/submissions/{submissionId}
        Task<SubmissionResultResponse> GetSubmissionResultAsync(int submissionId, int studentId);

        // Dùng cho API: GET /api/submissions/quiz/{quizId}
        Task<List<SubmissionSummaryResponse>> GetSubmissionHistoryAsync(int quizId, int studentId);
    }
}
