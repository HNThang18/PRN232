using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Response.Progress
{
    public class TopicProgressResponse
    {
        public string TopicName { get; set; }
        public int Completed { get; set; }
        public int Total { get; set; }
        public double ProgressPercent => Total > 0 ? (Completed * 100.0 / Total) : 0;
    }
}
