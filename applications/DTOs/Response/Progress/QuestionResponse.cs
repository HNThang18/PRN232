using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Response.Progress
{
    public class QuestionResponse
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; } // "MultipleChoice", "FillInTheBlank", ...

        // Chỉ chứa các lựa chọn trả lời, không chứa đáp án đúng
        public List<AnswerResponse> Answers { get; set; }
    }
}
