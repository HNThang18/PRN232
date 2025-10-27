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
        private readonly IQuizGenerationEventService _quizEventService;
        private readonly ILogger<AiController> _logger;

        public AiController(
            IAiIntegrationService aiIntegrationService,
            IAiService aiService,
            IQuizGenerationEventService quizEventService,
            ILogger<AiController> logger)
        {
            _aiIntegrationService = aiIntegrationService;
            _aiService = aiService;
            _quizEventService = quizEventService;
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
                _logger.LogInformation("Generating quiz with Event Sourcing: {Title}", request.Title);

                // Use Event Sourcing service instead of direct integration service
                var aggregate = await _quizEventService.GenerateQuizWithEventSourcingAsync(request, cancellationToken);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    aggregateId = aggregate.Id,
                    quizId = aggregate.QuizId,
                    title = aggregate.Title,
                    questionCount = aggregate.QuestionCount,
                    totalScore = aggregate.TotalScore,
                    timeLimit = aggregate.Duration,
                    status = aggregate.IsCompleted ? "Completed" : aggregate.IsFailed ? "Failed" : "Processing",
                    isCompleted = aggregate.IsCompleted,
                    isFailed = aggregate.IsFailed,
                    errorMessage = aggregate.ErrorMessage,
                    processingDuration = aggregate.ProcessingDuration,
                    eventCount = aggregate.Version,
                    createdAt = aggregate.InitiatedAt,
                    completedAt = aggregate.CompletedAt,
                    message = aggregate.IsCompleted 
                        ? "Quiz generated successfully with event sourcing" 
                        : aggregate.IsFailed 
                        ? $"Quiz generation failed: {aggregate.ErrorMessage}"
                        : "Quiz generation in progress"
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating quiz with event sourcing");
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

        [HttpGet("quizzes/generation/{aggregateId}")]
        public async Task<IActionResult> GetQuizGenerationHistoryAsync(string aggregateId)
        {
            try
            {
                _logger.LogInformation("Retrieving quiz generation history for aggregate: {AggregateId}", aggregateId);

                var aggregate = await _quizEventService.GetGenerationHistoryAsync(aggregateId);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    aggregateId = aggregate.Id,
                    version = aggregate.Version,
                    quizId = aggregate.QuizId,
                    title = aggregate.Title,
                    topic = aggregate.Topic,
                    gradeLevel = aggregate.GradeLevel,
                    questionCount = aggregate.QuestionCount,
                    totalScore = aggregate.TotalScore,
                    duration = aggregate.Duration,
                    teacherId = aggregate.TeacherId,
                    levelId = aggregate.LevelId,
                    aiRequestId = aggregate.AiRequestId,
                    aiRequestStatus = aggregate.AiRequestStatus?.ToString(),
                    isCompleted = aggregate.IsCompleted,
                    isFailed = aggregate.IsFailed,
                    errorMessage = aggregate.ErrorMessage,
                    initiatedAt = aggregate.InitiatedAt,
                    completedAt = aggregate.CompletedAt,
                    processingDuration = aggregate.ProcessingDuration,
                    questionIds = aggregate.QuestionIds,
                    eventTimeline = new
                    {
                        initiated = aggregate.InitiatedAt,
                        aiRequestCreated = aggregate.AiRequestId.HasValue,
                        contentGenerated = !string.IsNullOrEmpty(aggregate.AiResponse),
                        quizCreated = aggregate.QuizId.HasValue,
                        questionsAdded = aggregate.QuestionIds?.Any() == true,
                        completed = aggregate.CompletedAt
                    }
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quiz generation history");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpGet("quizzes/generation/{aggregateId}/exists")]
        public async Task<IActionResult> CheckGenerationExistsAsync(string aggregateId)
        {
            try
            {
                var exists = await _quizEventService.GenerationExistsAsync(aggregateId);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    aggregateId,
                    exists
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking generation existence");
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
