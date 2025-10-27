using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repositories.Models;
using services.Interfaces;

namespace controllers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonDetailController : ControllerBase
{
    private readonly ILessonDetailService _lessonDetailService;
    private readonly ILogger<LessonDetailController> _logger;

    public LessonDetailController(
        ILessonDetailService lessonDetailService,
        ILogger<LessonDetailController> logger)
    {
        _lessonDetailService = lessonDetailService;
        _logger = logger;
    }

    // ===== BASIC CRUD =====

    /// <summary>
    /// Get all lesson details
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonDetail>>> GetAllLessonDetails()
    {
        try
        {
            var lessonDetails = await _lessonDetailService.GetAllLessonDetailsAsync();
            return Ok(lessonDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all lesson details");
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách chi tiết bài học", error = ex.Message });
        }
    }

    /// <summary>
    /// Get lesson detail by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDetail>> GetLessonDetail(int id)
    {
        try
        {
            var lessonDetail = await _lessonDetailService.GetLessonDetailByIdAsync(id);
            if (lessonDetail == null)
                return NotFound(new { message = $"Không tìm thấy chi tiết bài học với ID {id}" });

            return Ok(lessonDetail);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting lesson detail {LessonDetailId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy chi tiết bài học", error = ex.Message });
        }
    }

    /// <summary>
    /// Get lesson detail with attachments
    /// </summary>
    [HttpGet("{id}/with-attachments")]
    public async Task<ActionResult<LessonDetail>> GetLessonDetailWithAttachments(int id)
    {
        try
        {
            var lessonDetail = await _lessonDetailService.GetLessonDetailWithAttachmentsAsync(id);
            if (lessonDetail == null)
                return NotFound(new { message = $"Không tìm thấy chi tiết bài học với ID {id}" });

            return Ok(lessonDetail);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting lesson detail {LessonDetailId} with attachments", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy chi tiết bài học", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new lesson detail
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<LessonDetail>> CreateLessonDetail([FromBody] LessonDetail lessonDetail)
    {
        try
        {
            var created = await _lessonDetailService.AddLessonDetailAsync(lessonDetail);
            return CreatedAtAction(nameof(GetLessonDetail), new { id = created.LessonDetailId }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating lesson detail");
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo chi tiết bài học", error = ex.Message });
        }
    }

    /// <summary>
    /// Update lesson detail
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateLessonDetail(int id, [FromBody] LessonDetail lessonDetail)
    {
        try
        {
            if (id != lessonDetail.LessonDetailId)
                return BadRequest(new { message = "ID không khớp" });

            await _lessonDetailService.UpdateLessonDetailAsync(lessonDetail);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating lesson detail {LessonDetailId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật chi tiết bài học", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete lesson detail
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteLessonDetail(int id)
    {
        try
        {
            await _lessonDetailService.DeleteLessonDetailAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting lesson detail {LessonDetailId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa chi tiết bài học", error = ex.Message });
        }
    }

    // ===== QUERY ENDPOINTS =====

    /// <summary>
    /// Get lesson details by lesson ID
    /// </summary>
    [HttpGet("lesson/{lessonId}")]
    public async Task<ActionResult<IEnumerable<LessonDetail>>> GetLessonDetailsByLesson(int lessonId)
    {
        try
        {
            var lessonDetails = await _lessonDetailService.GetLessonDetailsByLessonIdAsync(lessonId);
            return Ok(lessonDetails);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting lesson details for lesson {LessonId}", lessonId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách chi tiết bài học", error = ex.Message });
        }
    }

    /// <summary>
    /// Get lesson details by content type
    /// </summary>
    [HttpGet("content-type/{contentType}")]
    public async Task<ActionResult<IEnumerable<LessonDetail>>> GetLessonDetailsByContentType(ContentType contentType)
    {
        try
        {
            var lessonDetails = await _lessonDetailService.GetLessonDetailsByContentTypeAsync(contentType);
            return Ok(lessonDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting lesson details by content type {ContentType}", contentType);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách chi tiết bài học", error = ex.Message });
        }
    }

    /// <summary>
    /// Count lesson details by lesson ID
    /// </summary>
    [HttpGet("lesson/{lessonId}/count")]
    public async Task<ActionResult<int>> CountLessonDetailsByLesson(int lessonId)
    {
        try
        {
            var count = await _lessonDetailService.CountLessonDetailsByLessonIdAsync(lessonId);
            return Ok(new { count });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while counting lesson details for lesson {LessonId}", lessonId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi đếm chi tiết bài học", error = ex.Message });
        }
    }

    // ===== BUSINESS LOGIC ENDPOINTS =====

    /// <summary>
    /// Reorder lesson details (bulk update)
    /// </summary>
    [HttpPost("lesson/{lessonId}/reorder")]
    [Authorize]
    public async Task<IActionResult> ReorderLessonDetails(int lessonId, [FromBody] Dictionary<int, int> newOrders)
    {
        try
        {
            await _lessonDetailService.ReorderLessonDetailsAsync(lessonId, newOrders);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while reordering lesson details for lesson {LessonId}", lessonId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi sắp xếp lại chi tiết bài học", error = ex.Message });
        }
    }

    /// <summary>
    /// Check if lesson detail can be deleted
    /// </summary>
    [HttpGet("{id}/can-delete")]
    [Authorize]
    public async Task<ActionResult<object>> CanDeleteLessonDetail(int id)
    {
        try
        {
            var canDelete = await _lessonDetailService.CanDeleteLessonDetailAsync(id);
            return Ok(new { canDelete });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking if lesson detail {LessonDetailId} can be deleted", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi kiểm tra", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete all lesson details for a lesson
    /// </summary>
    [HttpDelete("lesson/{lessonId}/all")]
    [Authorize]
    public async Task<IActionResult> DeleteLessonDetailsByLesson(int lessonId)
    {
        try
        {
            await _lessonDetailService.DeleteLessonDetailsByLessonIdAsync(lessonId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting lesson details for lesson {LessonId}", lessonId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa chi tiết bài học", error = ex.Message });
        }
    }

    /// <summary>
    /// Duplicate a lesson detail
    /// </summary>
    [HttpPost("{id}/duplicate")]
    [Authorize]
    public async Task<ActionResult<LessonDetail>> DuplicateLessonDetail(int id, [FromQuery] int? targetLessonId = null)
    {
        try
        {
            var duplicate = await _lessonDetailService.DuplicateLessonDetailAsync(id, targetLessonId);
            return CreatedAtAction(nameof(GetLessonDetail), new { id = duplicate.LessonDetailId }, duplicate);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while duplicating lesson detail {LessonDetailId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi nhân bản chi tiết bài học", error = ex.Message });
        }
    }
}
