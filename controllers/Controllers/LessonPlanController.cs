using applications.DTOs.LessonPlan;
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
                var lessonPlanDtos = lessonPlans.Select(MapToDto).ToList();
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
                var lessonPlanDto = MapToDto(lessonPlan);
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
        }
    }
}
