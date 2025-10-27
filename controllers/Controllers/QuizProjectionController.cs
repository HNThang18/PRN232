using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using repositories.Dbcontext;
using repositories.EventSourcing.Projections;
using applications.DTOs.Response;
using services.EventSourcing.Projectors;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/quiz-projections")]
    [Authorize]
    public class QuizProjectionController : ControllerBase
    {
        private readonly MathLpContext _context;
        private readonly IQuizProjectorService _projectorService;
        private readonly ILogger<QuizProjectionController> _logger;

        public QuizProjectionController(
            MathLpContext context,
            IQuizProjectorService projectorService,
            ILogger<QuizProjectionController> logger)
        {
            _context = context;
            _projectorService = projectorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizList(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] int? gradeLevel = null)
        {
            try
            {
                var query = _context.QuizListProjections.AsQueryable();

                // Filters
                if (!string.IsNullOrEmpty(status))
                    query = query.Where(p => p.Status == status);

                if (teacherId.HasValue)
                    query = query.Where(p => p.TeacherId == teacherId.Value);

                if (gradeLevel.HasValue)
                    query = query.Where(p => p.GradeLevel == gradeLevel.Value);

                var total = await query.CountAsync();

                var items = await query
                    .OrderByDescending(p => p.InitiatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    items,
                    pagination = new
                    {
                        total,
                        page,
                        pageSize,
                        totalPages = (int)Math.Ceiling(total / (double)pageSize)
                    }
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz list from projection");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var stats = await _context.QuizStatisticsProjections
                    .FirstOrDefaultAsync();

                if (stats == null)
                {
                    return Ok(ApiResponse<object>.SuccessResponse(new
                    {
                        message = "No statistics available yet",
                        hasData = false
                    }));
                }

                return Ok(ApiResponse<QuizStatisticsProjection>.SuccessResponse(stats));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetTeacherDashboard(int teacherId)
        {
            try
            {
                var quizzes = await _context.QuizListProjections
                    .Where(p => p.TeacherId == teacherId)
                    .OrderByDescending(p => p.InitiatedAt)
                    .ToListAsync();

                var completedQuizzes = quizzes.Where(q => q.IsCompleted).ToList();

                var dashboard = new
                {
                    teacherId,
                    summary = new
                    {
                        totalQuizzes = quizzes.Count,
                        completedQuizzes = completedQuizzes.Count,
                        failedQuizzes = quizzes.Count(q => q.IsFailed),
                        processingQuizzes = quizzes.Count(q => !q.IsCompleted && !q.IsFailed),
                        totalQuestions = quizzes.Sum(q => q.QuestionCount),
                        averageDuration = completedQuizzes.Any()
                            ? completedQuizzes.Average(q => q.ProcessingDuration ?? 0)
                            : 0,
                        successRate = quizzes.Count > 0
                            ? (double)completedQuizzes.Count / quizzes.Count * 100
                            : 0
                    },
                    recentQuizzes = quizzes.Take(10).ToList(),
                    topicDistribution = quizzes
                        .GroupBy(q => q.Topic)
                        .Select(g => new { topic = g.Key, count = g.Count() })
                        .OrderByDescending(x => x.count)
                        .ToList(),
                    gradeLevelDistribution = quizzes
                        .GroupBy(q => q.GradeLevel)
                        .Select(g => new { gradeLevel = g.Key, count = g.Count() })
                        .OrderBy(x => x.gradeLevel)
                        .ToList()
                };

                return Ok(ApiResponse<object>.SuccessResponse(dashboard));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teacher dashboard for teacher {TeacherId}", teacherId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpGet("{aggregateId}")]
        public async Task<IActionResult> GetQuizProjection(string aggregateId)
        {
            try
            {
                var projection = await _context.QuizListProjections
                    .FirstOrDefaultAsync(p => p.AggregateId == aggregateId);

                if (projection == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse(404, "Quiz projection not found"));
                }

                return Ok(ApiResponse<QuizListProjection>.SuccessResponse(projection));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quiz projection {AggregateId}", aggregateId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }

        [HttpPost("rebuild")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RebuildProjections()
        {
            try
            {
                _logger.LogInformation("Starting projection rebuild...");

                await _projectorService.RebuildProjectionsAsync();

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    message = "Projections rebuilt successfully",
                    timestamp = DateTime.UtcNow
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rebuilding projections");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, 
                    $"Failed to rebuild projections: {ex.Message}"));
            }
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHealth()
        {
            try
            {
                var listCount = await _context.QuizListProjections.CountAsync();
                var statsExists = await _context.QuizStatisticsProjections.AnyAsync();
                var eventCount = await _context.EventStore
                    .Where(e => e.AggregateType == "QuizGeneration")
                    .CountAsync();

                var lastUpdate = await _context.QuizListProjections
                    .MaxAsync(p => (DateTime?)p.LastUpdated);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    status = "healthy",
                    projections = new
                    {
                        quizListCount = listCount,
                        statisticsExists = statsExists,
                        lastUpdate
                    },
                    eventStore = new
                    {
                        eventCount
                    }
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking projection health");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(500, ex.Message));
            }
        }
    }
}
