using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Response.Progress
{
    public class StudentProgressResponse
    {
        public double OverallScore { get; set; }
        public double ScoreChangePercent { get; set; }
        public int QuizzesCompleted { get; set; }
        public int TopicsMastered { get; set; }
    }
}
