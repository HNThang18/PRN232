using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Response.Progress
{
    public class AreaForImprovementResponse
    {
        public string TopicName { get; set; }
        public string Description { get; set; }
        public double AchievedPercent { get; set; }
    }
}
