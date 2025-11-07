using applications.DTOs.Request.AI;
using applications.DTOs.Response;
using applications.DTOs.Response.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repositories.Models;
using services.Interfaces;
using System.Security.Claims;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AiController : ControllerBase
    {
        private readonly IAiIntegrationService _aiIntegrationService;
        private readonly IAiService _aiService;
        private readonly ILogger<AiController> _logger;

        public AiController(
            IAiIntegrationService aiIntegrationService,
            IAiService aiService,
            ILogger<AiController> logger)
        {
            _aiIntegrationService = aiIntegrationService;
            _aiService = aiService;
            _logger = logger;
        }

        [HttpPost("lesson-plans/generate")]
        public async Task<IActionResult> GenerateLessonPlanAsync(
            [FromBody] AiLessonPlanRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                _logger.LogInformation("Generating lesson plan for topic: {Topic}", request.Topic);

                // Create a dedicated timeout for AI operations - don't link to request cancellation
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
                
                var lessonPlan = await _aiIntegrationService.GenerateAndSaveLessonPlanAsync(request, cts.Token);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    lessonPlanId = lessonPlan.LessonPlanId,
                    title = lessonPlan.Title,
                    topic = lessonPlan.Topic,
                    duration = lessonPlan.Duration,
                    status = lessonPlan.Status.ToString(),
                    createdAt = lessonPlan.CreatedAt,
                    message = "Lesson plan generated successfully"
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating lesson plan");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPost("lesson-plans/preview")]
        public async Task<IActionResult> PreviewLessonPlanAsync(
            [FromBody] AiLessonPlanRequestDto request, 
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                _logger.LogInformation("Previewing lesson plan for topic: {Topic}", request.Topic);

                var lessonPlanData = await _aiService.GenerateLessonPlanAsync(request, cancellationToken);

                return Ok(ApiResponse<AiLessonPlanResponseDto>.SuccessResponse(lessonPlanData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing lesson plan");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPost("questions/generate")]
        public async Task<IActionResult> GenerateQuestionsAsync(
            [FromBody] AiQuestionRequestDto request, 
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                _logger.LogInformation("Generating {Count} questions for topic: {Topic}", 
                    request.Count, request.Topic);

                var questions = await _aiIntegrationService.GenerateAndSaveQuestionsAsync(request, cancellationToken);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    count = questions.Count,
                    questionIds = questions.Select(q => q.QuestionId).ToList(),
                    message = $"Successfully generated {questions.Count} questions"
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating questions");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPost("questions/preview")]
        public async Task<IActionResult> PreviewQuestionsAsync(
            [FromBody] AiQuestionRequestDto request, 
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                _logger.LogInformation("Previewing {Count} questions for topic: {Topic}", 
                    request.Count, request.Topic);

                var questionsData = await _aiService.GenerateQuestionsAsync(request, cancellationToken);

                return Ok(ApiResponse<AiQuestionResponseDto>.SuccessResponse(questionsData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing questions");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPost("quizzes/generate")]
        public async Task<IActionResult> GenerateQuizAsync(
            [FromBody] AiQuizRequestDto request, 
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                _logger.LogInformation("Generating quiz: {Title}", request.Title);

                var quiz = await _aiIntegrationService.GenerateAndSaveQuizAsync(request, cancellationToken);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    quizId = quiz.QuizId,
                    title = quiz.Title,
                    questionCount = quiz.Questions?.Count ?? 0,
                    totalScore = quiz.TotalScore,
                    timeLimit = quiz.TimeLimit,
                    status = quiz.Status.ToString(),
                    createdAt = quiz.CreatedAt,
                    message = "Quiz generated successfully"
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating quiz");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPost("quizzes/preview")]
        public async Task<IActionResult> PreviewQuizAsync(
            [FromBody] AiQuizRequestDto request, 
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                _logger.LogInformation("Previewing quiz: {Title}", request.Title);

                var quizData = await _aiService.GenerateQuizAsync(request, cancellationToken);

                return Ok(ApiResponse<AiQuizResponseDto>.SuccessResponse(quizData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing quiz");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckHealthAsync(CancellationToken cancellationToken)
        {
            try
            {
                var isAvailable = await _aiService.IsAvailableAsync(cancellationToken);

                if (isAvailable)
                {
                    return Ok(ApiResponse<object>.SuccessResponse(new
                    {
                        status = "healthy",
                        message = "AI service is available"
                    }));
                }
                else
                {
                    return ServiceUnavailable(ApiResponse<object>.ErrorResponse(503, "AI service is not available"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking AI service health");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, "Failed to check AI service health"));
            }
        }

        [HttpPost("chat")]
        public async Task<IActionResult> ChatWithAiAsync(
            [FromBody] AiChatRequestDto request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                // Get user role from JWT claims
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "user";
                
                // Override the UserRole with the actual role from JWT
                request.UserRole = userRole.ToLower();
                
                _logger.LogInformation("Processing chat request from {Role}: {Message}", userRole, request.Message);

                var response = await _aiService.ChatAsync(request, cancellationToken);

                return Ok(ApiResponse<AiChatResponseDto>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat request");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpGet("requests/history")]
        public async Task<IActionResult> GetRequestHistoryAsync(
            [FromQuery] string? type = null,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(401, "User not authenticated"));
                }

                RequestType? requestType = null;
                if (!string.IsNullOrEmpty(type) && Enum.TryParse<RequestType>(type, true, out var parsedType))
                {
                    requestType = parsedType;
                }

                AiRequestStatus? requestStatus = null;
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<AiRequestStatus>(status, true, out var parsedStatus))
                {
                    requestStatus = parsedStatus;
                }

                var history = await _aiIntegrationService.GetRequestHistoryAsync(
                    userId, requestType, requestStatus, search, page, limit, cancellationToken);

                var total = await _aiIntegrationService.GetRequestCountAsync(
                    userId, requestType, requestStatus, search, cancellationToken);

                return Ok(new
                {
                    success = true,
                    data = history,
                    pagination = new
                    {
                        page,
                        limit,
                        total
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching request history");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        /// <summary>
        /// Get specific AI request details
        /// GET: api/ai/requests/{id}
        /// </summary>
        [HttpGet("requests/{id:int}")]
        public async Task<IActionResult> GetRequestDetailsAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var request = await _aiIntegrationService.GetRequestDetailsAsync(id, cancellationToken);

                if (request == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse(404, "Request not found"));
                }

                // Check if user owns this request
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(401, "User not authenticated"));
                }

                if (request.UserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                return Ok(ApiResponse<AiRequestDetailDto>.SuccessResponse(request));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching request details");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        private IActionResult ServiceUnavailable(object value)
        {
            return StatusCode(503, value);
        }
    }
}
