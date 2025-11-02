using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Response.Progress
{
    public class SubmissionDetailResponse
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string StudentAnswer { get; set; }
        public string CorrectAnswer { get; set; } // Đáp án đúng
        public bool IsCorrect { get; set; }
        public decimal ScoreEarned { get; set; }
        public string Explanation { get; set; } // Giải thích (nếu có)
    }
}