using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Response.Progress
{
    public class ProgressResponse
    {
        public int LessonId { get; set; }
        public string CompletionStatus { get; set; } // "Completed", "InProgress", "NotStarted"
        public DateTime? AttemptDate { get; set; }
    }
}
