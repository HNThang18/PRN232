using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Response.Progress
{
    public class SubmissionResultResponse
    {
        public int SubmissionId { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public decimal Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int DurationTaken { get; set; } // Thời gian làm bài (giây)
        public string Status { get; set; } // "Completed", "Failed", ...

        // Danh sách chi tiết các câu trả lời
        public List<SubmissionDetailResponse> Details { get; set; }
    }
}
