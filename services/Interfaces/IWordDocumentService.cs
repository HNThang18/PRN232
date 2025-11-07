using repositories.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services.Interfaces
{
    public interface IWordDocumentService
    {
        Task<MemoryStream> GenerateLessonPlanDocumentAsync(LessonPlan lessonPlan);
        Task<MemoryStream> GenerateLessonPlanWithLessonsDocumentAsync(LessonPlan lessonPlan);
    }
}
