using applications.DTOs.Quiz;
using applications.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repositories.Models;
using services.Interfaces;
using System.Security.Claims;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/quizzes")]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(
            IQuizService quizService,
            ILogger<QuizController> logger)
        {
            _quizService = quizService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchQuizzes(
            [FromQuery] string? keyword = null,
            [FromQuery] int? levelId = null,
            [FromQuery] string? status = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            try
            {
                QuizStatus? quizStatus = null;
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<QuizStatus>(status, true, out var parsedStatus))
                {
                    quizStatus = parsedStatus;
                }

                var quizzes = await _quizService.SearchQuizzesAsync(keyword, levelId, quizStatus, teacherId, page, limit);
                var total = await _quizService.GetSearchCountAsync(keyword, levelId, quizStatus, teacherId);

                return Ok(ApiResponse<object>.SuccessResponseWithPagination(quizzes, page, limit, total));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching quizzes");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpGet("my-quizzes")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyQuizzes(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int teacherId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(401, "User not authenticated"));
                }

                var quizzes = await _quizService.GetQuizzesByTeacherIdAsync(teacherId, page, limit);
                var total = await _quizService.GetQuizCountByTeacherIdAsync(teacherId, null);

                return Ok(ApiResponse<object>.SuccessResponseWithPagination(quizzes, page, limit, total));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teacher's quizzes");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetQuizById(int id)
        {
            try
            {
                var quiz = await _quizService.GetQuizDetailByIdAsync(id);
                if (quiz == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse(404, "Quiz not found"));
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole == "Teacher" && int.TryParse(userIdClaim, out int teacherId))
                {
                    if (quiz.TeacherId != teacherId)
                    {
                        return Forbid();
                    }
                }

                return Ok(ApiResponse<QuizDetailResponseDto>.SuccessResponse(quiz));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz by ID: {QuizId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int teacherId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(401, "User not authenticated"));
                }

                var quiz = await _quizService.CreateQuizAsync(teacherId, request);

                return CreatedAtAction(nameof(GetQuizById), new { id = quiz.QuizId }, 
                    ApiResponse<object>.SuccessResponse(new
                    {
                        quizId = quiz.QuizId,
                        title = quiz.Title,
                        message = "Quiz created successfully"
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromBody] UpdateQuizRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int teacherId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(401, "User not authenticated"));
                }

                var quiz = await _quizService.UpdateQuizAsync(id, teacherId, request);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    quizId = quiz.QuizId,
                    title = quiz.Title,
                    message = "Quiz updated successfully"
                }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(404, ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quiz: {QuizId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int teacherId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(401, "User not authenticated"));
                }

                await _quizService.DeleteQuizAsync(id, teacherId);

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Quiz deleted successfully" }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(404, ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quiz: {QuizId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPost("{id:int}/publish")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> PublishQuiz(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int teacherId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(401, "User not authenticated"));
                }

                await _quizService.PublishQuizAsync(id, teacherId);

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Quiz published successfully" }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(404, ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing quiz: {QuizId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPost("{id:int}/unpublish")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> UnpublishQuiz(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int teacherId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(401, "User not authenticated"));
                }

                await _quizService.UnpublishQuizAsync(id, teacherId);

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Quiz unpublished successfully" }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(404, ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing quiz: {QuizId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpGet("{id:int}/statistics")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> GetQuizStatistics(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                var quiz = await _quizService.GetQuizByIdAsync(id);
                if (quiz == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse(404, "Quiz not found"));
                }

                if (userRole == "Teacher" && int.TryParse(userIdClaim, out int teacherId))
                {
                    if (quiz.TeacherId != teacherId)
                    {
                        return Forbid();
                    }
                }

                var statistics = await _quizService.GetQuizStatisticsAsync(id);

                return Ok(ApiResponse<QuizStatisticsDto>.SuccessResponse(statistics));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz statistics: {QuizId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPost("{id:int}/questions")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddQuestionsToQuiz(int id, [FromBody] AddQuestionsToQuizRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int teacherId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(401, "User not authenticated"));
                }

                await _quizService.AddQuestionsToQuizAsync(id, teacherId, request.QuestionIds);

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Questions added successfully" }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(404, ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding questions to quiz: {QuizId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpDelete("{id:int}/questions/{questionId:int}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> RemoveQuestionFromQuiz(int id, int questionId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int teacherId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(401, "User not authenticated"));
                }

                await _quizService.RemoveQuestionFromQuizAsync(id, teacherId, questionId);

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Question removed successfully" }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(404, ex.Message));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing question from quiz: {QuizId}, {QuestionId}", id, questionId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }
    }
}
