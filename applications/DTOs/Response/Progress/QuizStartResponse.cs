using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Response.Progress
{
    public class QuizStartResponse
    {
        public int SubmissionId { get; set; } // ID của lần làm bài này
        public int TimeLimit { get; set; } // Thời gian làm bài (phút)
        public List<QuestionResponse> Questions { get; set; } // Danh sách câu hỏi
    }
}
