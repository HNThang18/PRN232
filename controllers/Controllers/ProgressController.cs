using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using services.Interfaces;
using System.Security.Claims;

namespace controllers.Controllers
{
    [Route("api/progress")]
    [ApiController]
    [Authorize] // BẮT BUỘC: Yêu cầu người dùng phải đăng nhập
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        // Hàm private tiện ích để lấy studentId, tránh lặp code
        private int GetCurrentStudentId()
        {
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (studentIdClaim == null || !int.TryParse(studentIdClaim.Value, out int studentId))
            {
                // Ném lỗi hoặc trả về 0 tùy theo cách xử lý của bạn
                throw new UnauthorizedAccessException("Không thể xác định ID người dùng.");
            }
            return studentId;
        }

        /// <summary>
        /// Đánh dấu một bài học là "Đang học".
        /// </summary>
        [HttpPost("start/{lessonId}")]
        public async Task<IActionResult> MarkAsInProgress(int lessonId)
        {
            var studentId = GetCurrentStudentId();
            var result = await _progressService.MarkLessonAsInProgress(lessonId, studentId);

            if (!result)
            {
                return BadRequest("Không thể cập nhật tiến độ.");
            }
            return Ok("Đã đánh dấu bài học đang được theo dõi.");
        }

        /// <summary>
        /// Đánh dấu một bài học là "Đã hoàn thành".
        /// </summary>
        [HttpPost("complete/{lessonId}")]
        public async Task<IActionResult> MarkAsCompleted(int lessonId)
        {
            var studentId = GetCurrentStudentId();
            var result = await _progressService.MarkLessonAsCompleted(lessonId, studentId);

            if (!result)
            {
                return BadRequest("Không thể cập nhật tiến độ.");
            }
            return Ok("Đã cập nhật tiến độ thành công.");
        }

        /// <summary>
        /// Lấy tiến độ của tất cả bài học trong một kế hoạch bài giảng.
        /// </summary>
        [HttpGet("lesson-plan/{lessonPlanId}")]
        public async Task<IActionResult> GetLessonPlanProgress(int lessonPlanId)
        {
            var studentId = GetCurrentStudentId();
            var progressList = await _progressService.GetLessonPlanProgressAsync(lessonPlanId, studentId);
            return Ok(progressList);
        }
    }
}
