using applications.DTOs.Request.Progress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using services.Interfaces;
using System.Security.Claims;

namespace controllers.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    [Authorize] // BẮT BUỘC: Yêu cầu người dùng phải đăng nhập
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionsController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        // Hàm private tiện ích để lấy studentId
        private int GetCurrentStudentId()
        {
            var studentIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (studentIdClaim == null || !int.TryParse(studentIdClaim.Value, out int studentId))
            {
                throw new UnauthorizedAccessException("Không thể xác định ID người dùng.");
            }
            return studentId;
        }

        /// <summary>
        /// Bắt đầu một lượt làm bài quiz.
        /// </summary>
        [HttpPost("start/{quizId}")]
        public async Task<IActionResult> StartQuiz(int quizId)
        {
            var studentId = GetCurrentStudentId();
            // Dùng try-catch để bắt các lỗi nghiệp vụ (ví dụ: hết lượt)
            try
            {
                var response = await _submissionService.StartQuizAsync(quizId, studentId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Nộp bài và chấm điểm.
        /// </summary>
        [HttpPost("{submissionId}/submit")]
        public async Task<IActionResult> SubmitQuiz(int submissionId, [FromBody] SubmitQuizRequest request)
        {
            var studentId = GetCurrentStudentId();
            try
            {
                var result = await _submissionService.SubmitQuizAsync(submissionId, request, studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lấy kết quả chi tiết của một lần nộp bài.
        /// </summary>
        [HttpGet("{submissionId}")]
        public async Task<IActionResult> GetSubmissionResult(int submissionId)
        {
            var studentId = GetCurrentStudentId();
            try
            {
                var result = await _submissionService.GetSubmissionResultAsync(submissionId, studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lấy lịch sử làm bài của một quiz.
        /// </summary>
        [HttpGet("quiz/{quizId}")]
        public async Task<IActionResult> GetSubmissionHistory(int quizId)
        {
            var studentId = GetCurrentStudentId();
            var history = await _submissionService.GetSubmissionHistoryAsync(quizId, studentId);
            return Ok(history);
        }
    }
}
