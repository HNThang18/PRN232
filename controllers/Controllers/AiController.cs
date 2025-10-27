using applications.DTOs.Request.AI;
using applications.DTOs.Response;
using applications.DTOs.Response.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using services.Interfaces;

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
            [FromBody] AiLessonPlanRequestDto request, 
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, "Invalid request data"));
            }

            try
            {
                _logger.LogInformation("Generating lesson plan for topic: {Topic}", request.Topic);

                var lessonPlan = await _aiIntegrationService.GenerateAndSaveLessonPlanAsync(request, cancellationToken);

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

        private IActionResult ServiceUnavailable(object value)
        {
            return StatusCode(503, value);
        }
    }
}
