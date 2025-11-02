using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Request.Progress
{
    public class SubmitQuizRequest
    {
        public List<StudentAnswer> Answers { get; set; }
    }
}
