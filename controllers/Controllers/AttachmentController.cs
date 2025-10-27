using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using repositories.Models;
using services.Interfaces;
using System.Security.Claims;

namespace controllers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly ILogger<AttachmentController> _logger;

    public AttachmentController(
        IAttachmentService attachmentService,
        ILogger<AttachmentController> logger)
    {
        _attachmentService = attachmentService;
        _logger = logger;
    }

    // ===== BASIC CRUD =====

    /// <summary>
    /// Get all attachments
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Attachment>>> GetAllAttachments()
    {
        try
        {
            var attachments = await _attachmentService.GetAllAttachmentsAsync();
            return Ok(attachments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all attachments");
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách file đính kèm", error = ex.Message });
        }
    }

    /// <summary>
    /// Get attachment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Attachment>> GetAttachment(int id)
    {
        try
        {
            var attachment = await _attachmentService.GetAttachmentByIdAsync(id);
            if (attachment == null)
                return NotFound(new { message = $"Không tìm thấy file đính kèm với ID {id}" });

            return Ok(attachment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting attachment {AttachmentId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy file đính kèm", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new attachment
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Attachment>> CreateAttachment([FromBody] Attachment attachment)
    {
        try
        {
            // Get current user ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                attachment.UploadedBy = userId;
            }

            var createdAttachment = await _attachmentService.AddAttachmentAsync(attachment);
            return CreatedAtAction(nameof(GetAttachment), new { id = createdAttachment.AttachmentId }, createdAttachment);
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
            _logger.LogError(ex, "Error occurred while creating attachment");
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo file đính kèm", error = ex.Message });
        }
    }

    /// <summary>
    /// Update attachment
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAttachment(int id, [FromBody] Attachment attachment)
    {
        try
        {
            if (id != attachment.AttachmentId)
                return BadRequest(new { message = "ID không khớp" });

            await _attachmentService.UpdateAttachmentAsync(attachment);
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
            _logger.LogError(ex, "Error occurred while updating attachment {AttachmentId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật file đính kèm", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete attachment
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAttachment(int id)
    {
        try
        {
            await _attachmentService.DeleteAttachmentAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting attachment {AttachmentId}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa file đính kèm", error = ex.Message });
        }
    }

    // ===== QUERY ENDPOINTS =====

    /// <summary>
    /// Get attachments by LessonDetail ID
    /// </summary>
    [HttpGet("lessondetail/{lessonDetailId}")]
    public async Task<ActionResult<IEnumerable<Attachment>>> GetAttachmentsByLessonDetail(int lessonDetailId)
    {
        try
        {
            var attachments = await _attachmentService.GetAttachmentsByLessonDetailIdAsync(lessonDetailId);
            return Ok(attachments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting attachments for LessonDetail {LessonDetailId}", lessonDetailId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách file đính kèm", error = ex.Message });
        }
    }

    /// <summary>
    /// Get attachments by file type
    /// </summary>
    [HttpGet("filetype/{fileType}")]
    public async Task<ActionResult<IEnumerable<Attachment>>> GetAttachmentsByFileType(string fileType)
    {
        try
        {
            var attachments = await _attachmentService.GetAttachmentsByFileTypeAsync(fileType);
            return Ok(attachments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting attachments by file type {FileType}", fileType);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách file đính kèm", error = ex.Message });
        }
    }

    /// <summary>
    /// Get attachments by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Attachment>>> GetAttachmentsByUser(int userId)
    {
        try
        {
            var attachments = await _attachmentService.GetAttachmentsByUserIdAsync(userId);
            return Ok(attachments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting attachments for user {UserId}", userId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách file đính kèm", error = ex.Message });
        }
    }

    /// <summary>
    /// Count attachments by LessonDetail ID
    /// </summary>
    [HttpGet("lessondetail/{lessonDetailId}/count")]
    public async Task<ActionResult<int>> CountAttachmentsByLessonDetail(int lessonDetailId)
    {
        try
        {
            var count = await _attachmentService.CountAttachmentsByLessonDetailIdAsync(lessonDetailId);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while counting attachments for LessonDetail {LessonDetailId}", lessonDetailId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi đếm file đính kèm", error = ex.Message });
        }
    }

    // ===== BUSINESS LOGIC ENDPOINTS =====

    /// <summary>
    /// Get total file size by LessonDetail ID
    /// </summary>
    [HttpGet("lessondetail/{lessonDetailId}/total-size")]
    public async Task<ActionResult<object>> GetTotalFileSizeByLessonDetail(int lessonDetailId)
    {
        try
        {
            var totalSize = await _attachmentService.GetTotalFileSizeByLessonDetailIdAsync(lessonDetailId);
            var formattedSize = await _attachmentService.GetFormattedFileSizeAsync(totalSize);

            return Ok(new
            {
                totalBytes = totalSize,
                formattedSize = formattedSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting total file size for LessonDetail {LessonDetailId}", lessonDetailId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi tính tổng dung lượng file", error = ex.Message });
        }
    }

    /// <summary>
    /// Get total file size by User ID
    /// </summary>
    [HttpGet("user/{userId}/total-size")]
    [Authorize]
    public async Task<ActionResult<object>> GetTotalFileSizeByUser(int userId)
    {
        try
        {
            var totalSize = await _attachmentService.GetTotalFileSizeByUserIdAsync(userId);
            var formattedSize = await _attachmentService.GetFormattedFileSizeAsync(totalSize);

            return Ok(new
            {
                totalBytes = totalSize,
                formattedSize = formattedSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting total file size for user {UserId}", userId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi tính tổng dung lượng file", error = ex.Message });
        }
    }

    /// <summary>
    /// Validate file size
    /// </summary>
    [HttpGet("validate/size")]
    public async Task<ActionResult<object>> ValidateFileSize([FromQuery] long fileSize, [FromQuery] long? maxFileSize = null)
    {
        try
        {
            var maxSize = maxFileSize ?? 10 * 1024 * 1024; // Default 10MB
            var isValid = await _attachmentService.ValidateFileSize(fileSize, maxSize);
            var formattedSize = await _attachmentService.GetFormattedFileSizeAsync(fileSize);
            var formattedMaxSize = await _attachmentService.GetFormattedFileSizeAsync(maxSize);

            return Ok(new
            {
                isValid,
                fileSize = formattedSize,
                maxFileSize = formattedMaxSize,
                fileSizeBytes = fileSize,
                maxFileSizeBytes = maxSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while validating file size");
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi kiểm tra kích thước file", error = ex.Message });
        }
    }

    /// <summary>
    /// Validate file type
    /// </summary>
    [HttpGet("validate/type")]
    public async Task<ActionResult<object>> ValidateFileType([FromQuery] string fileType, [FromQuery] string[]? allowedTypes = null)
    {
        try
        {
            var defaultTypes = new[] { "pdf", "doc", "docx", "jpg", "jpeg", "png", "gif", "mp4", "mp3" };
            var typesToValidate = allowedTypes ?? defaultTypes;
            var isValid = await _attachmentService.ValidateFileType(fileType, typesToValidate);

            return Ok(new
            {
                isValid,
                fileType,
                allowedTypes = typesToValidate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while validating file type");
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi kiểm tra loại file", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete all attachments by LessonDetail ID
    /// </summary>
    [HttpDelete("lessondetail/{lessonDetailId}/all")]
    [Authorize]
    public async Task<IActionResult> DeleteAttachmentsByLessonDetail(int lessonDetailId)
    {
        try
        {
            await _attachmentService.DeleteAttachmentsByLessonDetailIdAsync(lessonDetailId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting attachments for LessonDetail {LessonDetailId}", lessonDetailId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa file đính kèm", error = ex.Message });
        }
    }
}
