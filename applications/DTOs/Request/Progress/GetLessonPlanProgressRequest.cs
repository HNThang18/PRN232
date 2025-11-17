using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Request.Progress
{
    public class GetLessonPlanProgressRequest
    {
        public int StudentId { get; set; }
        public int LessonPlanId { get; set; }
    }
}
