using applications.DTOs.LessonPlan;
using applications.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using repositories.Models;
using services.Interfaces;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/lesson-plans")]
    public class LessonPlanController : ControllerBase
    {
        private readonly ILessonPlanService _lessonPlanService;

        public LessonPlanController(ILessonPlanService lessonPlanService)
        {
            _lessonPlanService = lessonPlanService;
        }

        // ===== HELPER METHODS =====

        private static LessonPlanResponseDto MapToResponseDto(LessonPlan lp)
        {
            return new LessonPlanResponseDto
            {
                LessonPlanId = lp.LessonPlanId,
                Title = lp.Title,
                Topic = lp.Topic,
                Description = lp.LearningObjectives, // Map LearningObjectives to Description
                EstimatedDuration = lp.Duration, // Map Duration to EstimatedDuration
                Status = lp.Status.ToString(),
                IsPublic = lp.IsPublic,
                Version = lp.Version,
                PublishedDate = lp.CreatedAt, // LessonPlan doesn't have PublishedDate, use CreatedAt
                CreatedAt = lp.CreatedAt,
                UpdatedAt = lp.UpdatedAt,
                TeacherId = lp.TeacherId,
                LevelId = lp.LevelId,
                AiRequestId = lp.AiRequestId,
                TeacherName = lp.Teacher?.Username,
                LevelName = lp.Level?.LevelName,
                LessonsCount = lp.Lessons?.Count ?? 0
            };
        }

        private static LessonPlanWithLessonsDto MapToWithLessonsDto(LessonPlan lp)
        {
            return new LessonPlanWithLessonsDto
            {
                LessonPlanId = lp.LessonPlanId,
                Title = lp.Title,
                Topic = lp.Topic,
                Description = lp.LearningObjectives,
                EstimatedDuration = lp.Duration,
                Status = lp.Status.ToString(),
                IsPublic = lp.IsPublic,
                Version = lp.Version,
                PublishedDate = lp.CreatedAt,
                CreatedAt = lp.CreatedAt,
                UpdatedAt = lp.UpdatedAt,
                TeacherId = lp.TeacherId,
                LevelId = lp.LevelId,
                AiRequestId = lp.AiRequestId,
                TeacherName = lp.Teacher?.Username,
                LevelName = lp.Level?.LevelName,
                LessonsCount = lp.Lessons?.Count ?? 0,
                Lessons = lp.Lessons?.Select(l => new LessonResponseDto
                {
                    LessonId = l.LessonId,
                    Title = l.Title,
                    Content = l.Content,
                    Order = l.Order,
                    IsShared = l.IsShared,
                    PublishedDate = l.PublishedDate,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt,
                    LessonPlanId = l.LessonPlanId,
                    LessonPlanTitle = l.LessonPlan?.Title,
                    LessonDetailsCount = l.LessonDetails?.Count ?? 0,
                    ProgressCount = l.Progresses?.Count ?? 0
                }).ToList() ?? new List<LessonResponseDto>()
            };
        }        // Keep old mapper for backward compatibility with CreateLessonPlanDto
        private static LessonPlanDto MapToDto(LessonPlan lp)
        {
            return new LessonPlanDto
            {
                LessonPlanId = lp.LessonPlanId,
                TeacherId = lp.TeacherId,
                TeacherName = lp.Teacher?.Username,
                LevelId = lp.LevelId,
                LevelName = lp.Level?.LevelName,
                TopicName = lp.Topic,
                Title = lp.Title,
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLessonPlansAsync(CancellationToken cancellationToken)
        {
            try
            {
                var lessonPlans = await _lessonPlanService.GetAllLessonPlansAsync(cancellationToken);
                var lessonPlanDtos = lessonPlans.Select(MapToResponseDto).ToList();
                return Ok(new { success = true, data = lessonPlanDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLessonPlanByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var lessonPlan = await _lessonPlanService.GetLessonPlanByIdAsync(id, cancellationToken);
                var lessonPlanDto = MapToResponseDto(lessonPlan!);
                return Ok(new { success = true, data = lessonPlanDto });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = new { code = 404, message = ex.Message } });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateLessonPlanAsync([FromBody] CreateLessonPlanDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = "Invalid request data" } });
            }

            try
            {
                var lessonPlan = new LessonPlan
                {
                    TeacherId = dto.TeacherId,
                    LevelId = dto.LevelId,
                    Topic = dto.TopicName ?? string.Empty,
                    Title = dto.Title,
                    CreatedAt = DateTime.UtcNow,
                    Status = LessonPlanStatus.Draft
                };

                await _lessonPlanService.AddLessonPlanAsync(lessonPlan, cancellationToken);
                var lessonPlanDto = MapToDto(lessonPlan);
                return CreatedAtAction(nameof(GetLessonPlanByIdAsync), new { id = lessonPlan.LessonPlanId },
                    new { success = true, data = lessonPlanDto });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateLessonPlanAsync(int id, [FromBody] LessonPlanUpdateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = "Invalid request data" } });
            }

            try
            {
                var lessonPlan = new LessonPlan
                {
                    LessonPlanId = id,
                    TeacherId = dto.TeacherId,
                    LevelId = dto.LevelId,
                    Topic = dto.TopicName ?? string.Empty,
                    Title = dto.Title,
                    UpdatedAt = DateTime.UtcNow
                };

                await _lessonPlanService.UpdateLessonPlanAsync(lessonPlan, cancellationToken);
                return Ok(new { success = true, message = "LessonPlan updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = new { code = 404, message = ex.Message } });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLessonPlanAsync(int id, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = "Invalid request data" } });
            }
            try
            {
                await _lessonPlanService.DeleteLessonPlanAsync(id, cancellationToken);
                return Ok(new { success = true, message = "LessonPlan deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = new { code = 404, message = ex.Message } });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, error = new { code = 409, message = ex.Message } });
            }
        }

        // ================== QUERY ENDPOINTS ==================

        /// <summary>
        /// Lấy lesson plan với đầy đủ lessons và details
        /// GET: api/lesson-plans/{id}/with-lessons
        /// </summary>
        [HttpGet("{id:int}/with-lessons")]
        public async Task<IActionResult> GetLessonPlanWithLessonsAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var lessonPlan = await _lessonPlanService.GetLessonPlanWithLessonsAsync(id, cancellationToken);
                if (lessonPlan == null)
                {
                    return NotFound(new { success = false, error = new { code = 404, message = "Lesson plan not found." } });
                }
                var dto = MapToWithLessonsDto(lessonPlan);
                return Ok(new { success = true, data = dto });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = new { code = 404, message = ex.Message } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        /// <summary>
        /// Lấy tất cả lesson plans của một teacher
        /// GET: api/lesson-plans/teacher/{teacherId}
        /// </summary>
        [HttpGet("teacher/{teacherId:int}")]
        public async Task<IActionResult> GetLessonPlansByTeacherAsync(int teacherId, CancellationToken cancellationToken)
        {
            try
            {
                var lessonPlans = await _lessonPlanService.GetLessonPlansByTeacherIdAsync(teacherId, cancellationToken);
                var lessonPlanDtos = lessonPlans.Select(MapToResponseDto).ToList();
                return Ok(new { success = true, data = lessonPlanDtos });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        /// <summary>
        /// Lấy tất cả lesson plans của một level
        /// GET: api/lesson-plans/level/{levelId}
        /// </summary>
        [HttpGet("level/{levelId:int}")]
        public async Task<IActionResult> GetLessonPlansByLevelAsync(int levelId, CancellationToken cancellationToken)
        {
            try
            {
                var lessonPlans = await _lessonPlanService.GetLessonPlansByLevelIdAsync(levelId, cancellationToken);
                var lessonPlanDtos = lessonPlans.Select(MapToResponseDto).ToList();
                return Ok(new { success = true, data = lessonPlanDtos });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        /// <summary>
        /// Lấy tất cả lesson plans công khai
        /// GET: api/lesson-plans/public
        /// </summary>
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicLessonPlansAsync(CancellationToken cancellationToken)
        {
            try
            {
                var lessonPlans = await _lessonPlanService.GetPublicLessonPlansAsync(cancellationToken);
                var lessonPlanDtos = lessonPlans.Select(MapToResponseDto).ToList();
                return Ok(new { success = true, data = lessonPlanDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        /// <summary>
        /// Tìm kiếm lesson plans theo keyword (title, topic, tags)
        /// GET: api/lesson-plans/search?keyword=algebra
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchLessonPlansAsync([FromQuery] string keyword, CancellationToken cancellationToken)
        {
            try
            {
                var lessonPlans = await _lessonPlanService.SearchLessonPlansAsync(keyword, cancellationToken);
                var lessonPlanDtos = lessonPlans.Select(MapToDto).ToList();
                return Ok(new { success = true, data = lessonPlanDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        // ================== BUSINESS ACTIONS ==================

        /// <summary>
        /// Publish lesson plan (make it available publicly)
        /// POST: api/lesson-plans/{id}/publish
        /// </summary>
        [HttpPost("{id:int}/publish")]
        public async Task<IActionResult> PublishLessonPlanAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _lessonPlanService.PublishLessonPlanAsync(id, cancellationToken);
                return Ok(new { success = true, message = "LessonPlan published successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = new { code = 404, message = ex.Message } });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        /// <summary>
        /// Unpublish lesson plan (revert to draft)
        /// POST: api/lesson-plans/{id}/unpublish
        /// </summary>
        [HttpPost("{id:int}/unpublish")]
        public async Task<IActionResult> UnpublishLessonPlanAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _lessonPlanService.UnpublishLessonPlanAsync(id, cancellationToken);
                return Ok(new { success = true, message = "LessonPlan unpublished successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = new { code = 404, message = ex.Message } });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        /// <summary>
        /// Duplicate/Clone lesson plan cho teacher khác
        /// POST: api/lesson-plans/{id}/duplicate
        /// Body: { "newTeacherId": 5 }
        /// </summary>
        [HttpPost("{id:int}/duplicate")]
        public async Task<IActionResult> DuplicateLessonPlanAsync(int id, [FromBody] DuplicateLessonPlanDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = "Invalid request data" } });
            }

            try
            {
                await _lessonPlanService.DuplicateLessonPlanAsync(id, dto.NewTeacherId, cancellationToken);
                return Ok(new { success = true, message = "LessonPlan duplicated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = new { code = 404, message = ex.Message } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        /// <summary>
        /// Kiểm tra có thể xóa lesson plan không
        /// GET: api/lesson-plans/{id}/can-delete
        /// </summary>
        [HttpGet("{id:int}/can-delete")]
        public async Task<IActionResult> CanDeleteLessonPlanAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var canDelete = await _lessonPlanService.CanDeleteLessonPlanAsync(id, cancellationToken);
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        canDelete,
                        message = canDelete
                            ? "Có thể xóa lesson plan này"
                            : "Không thể xóa vì đã published hoặc có student progress"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }
        [HttpGet("{id:int}/download")]
        public async Task<IActionResult> GetLessonPlanDownloadUrlAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var lessonPlan = await _lessonPlanService.GetLessonPlanByIdAsync(id, cancellationToken);
                
                if (lessonPlan == null)
                {
                    return NotFound(new { success = false, error = new { code = 404, message = "Lesson plan not found" } });
                }

                if (string.IsNullOrEmpty(lessonPlan.ExportPath))
                {
                    return NotFound(new { success = false, error = new { code = 404, message = "Word document not available for this lesson plan" } });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        downloadUrl = lessonPlan.ExportPath,
                        fileName = $"{lessonPlan.Title}.docx",
                        lessonPlanId = lessonPlan.LessonPlanId,
                        title = lessonPlan.Title
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, error = new { code = 400, message = ex.Message } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }
    }
}
