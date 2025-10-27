using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using repositories.Models;
using services.Interfaces;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/lessons")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        // ================== BASIC CRUD ==================

        /// <summary>
        /// Lấy tất cả lessons
        /// GET: api/lessons
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllLessonsAsync()
        {
            try
            {
                var lessons = await _lessonService.GetAllLessonsAsync();
                return Ok(new { success = true, data = lessons });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        /// <summary>
        /// Lấy lesson theo ID (không include details)
        /// GET: api/lessons/{id}
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLessonByIdAsync(int id)
        {
            try
            {
                var lesson = await _lessonService.GetLessonByIdAsync(id);
                if (lesson == null)
                {
                    return NotFound(new { success = false, error = new { code = 404, message = "Lesson not found." } });
                }
                return Ok(new { success = true, data = lesson });
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
        /// Lấy lesson với đầy đủ details và attachments
        /// GET: api/lessons/{id}/details
        /// </summary>
        [HttpGet("{id:int}/details")]
        public async Task<IActionResult> GetLessonWithDetailsAsync(int id)
        {
            try
            {
                var lesson = await _lessonService.GetLessonWithDetailsAsync(id);
                return Ok(new { success = true, data = lesson });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = new { code = 404, message = ex.Message } });
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
        /// Tạo lesson mới
        /// POST: api/lessons
        /// Body: { "lessonPlanId": 1, "title": "Bài học 1", "content": "...", "order": 1 (optional) }
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateLessonAsync([FromBody] Lesson lesson)
        {
            try
            {
                var createdLesson = await _lessonService.AddLessonAsync(lesson);
                return CreatedAtAction(
                    nameof(GetLessonByIdAsync),
                    new { id = createdLesson.LessonId },
                    new { success = true, data = createdLesson }
                );
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
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        /// <summary>
        /// Cập nhật lesson
        /// PUT: api/lessons/{id}
        /// Body: { "lessonId": 1, "lessonPlanId": 1, "title": "Updated", ... }
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateLessonAsync(int id, [FromBody] Lesson lesson)
        {
            try
            {
                if (id != lesson.LessonId)
                {
                    return BadRequest(new { success = false, error = new { code = 400, message = "ID không khớp" } });
                }

                await _lessonService.UpdateLessonAsync(lesson);
                return Ok(new { success = true, message = "Cập nhật lesson thành công" });
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
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        /// <summary>
        /// Xóa lesson
        /// DELETE: api/lessons/{id}
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLessonAsync(int id)
        {
            try
            {
                await _lessonService.DeleteLessonAsync(id);
                return Ok(new { success = true, message = "Xóa lesson thành công" });
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
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        // ================== QUERY ENDPOINTS ==================

        /// <summary>
        /// Lấy tất cả lessons của một giáo án
        /// GET: api/lessons/lessonplan/{lessonPlanId}
        /// </summary>
        [HttpGet("lessonplan/{lessonPlanId:int}")]
        public async Task<IActionResult> GetLessonsByLessonPlanIdAsync(int lessonPlanId)
        {
            try
            {
                var lessons = await _lessonService.GetLessonsByLessonPlanIdAsync(lessonPlanId);
                return Ok(new { success = true, data = lessons });
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
        /// Lấy tất cả lessons được chia sẻ công khai
        /// GET: api/lessons/shared
        /// </summary>
        [HttpGet("shared")]
        public async Task<IActionResult> GetSharedLessonsAsync()
        {
            try
            {
                var lessons = await _lessonService.GetSharedLessonsAsync();
                return Ok(new { success = true, data = lessons });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }

        // ================== BUSINESS ACTIONS ==================

        /// <summary>
        /// Publish lesson (share publicly)
        /// POST: api/lessons/{id}/publish
        /// </summary>
        [HttpPost("{id:int}/publish")]
        public async Task<IActionResult> PublishLessonAsync(int id)
        {
            try
            {
                await _lessonService.PublishLessonAsync(id);
                return Ok(new { success = true, message = "Lesson đã được publish" });
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
        /// Unpublish lesson (make private)
        /// POST: api/lessons/{id}/unpublish
        /// </summary>
        [HttpPost("{id:int}/unpublish")]
        public async Task<IActionResult> UnpublishLessonAsync(int id)
        {
            try
            {
                await _lessonService.UnpublishLessonAsync(id);
                return Ok(new { success = true, message = "Lesson đã được unpublish" });
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
        /// Reorder lessons trong một giáo án
        /// POST: api/lessons/lessonplan/{lessonPlanId}/reorder
        /// Body: { "1": 3, "2": 1, "3": 2 } (LessonId: NewOrder)
        /// </summary>
        [HttpPost("lessonplan/{lessonPlanId:int}/reorder")]
        public async Task<IActionResult> ReorderLessonsAsync(int lessonPlanId, [FromBody] Dictionary<int, int> newOrders)
        {
            try
            {
                await _lessonService.ReorderLessonsAsync(lessonPlanId, newOrders);
                return Ok(new { success = true, message = "Reorder lessons thành công" });
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
        /// Kiểm tra có thể xóa lesson không
        /// GET: api/lessons/{id}/can-delete
        /// </summary>
        [HttpGet("{id:int}/can-delete")]
        public async Task<IActionResult> CanDeleteLessonAsync(int id)
        {
            try
            {
                var canDelete = await _lessonService.CanDeleteLessonAsync(id);
                return Ok(new { success = true, data = new { canDelete, message = canDelete ? "Có thể xóa" : "Không thể xóa vì có học sinh đang học" } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = new { code = 500, message = ex.Message } });
            }
        }
    }
}