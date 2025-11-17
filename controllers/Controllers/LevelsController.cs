using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using applications.DTOs.Response;
using applications.DTOs.Level;
using services.Interfaces;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LevelsController : ControllerBase
    {
        private readonly ILevelService _levelService;

        public LevelsController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<LevelResponseDto>>>> GetAllLevels()
        {
            try
            {
                var levels = await _levelService.GetAllLevelsAsync();
                return Ok(ApiResponse<IEnumerable<LevelResponseDto>>.SuccessResponse(levels));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<LevelResponseDto>>.ErrorResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<LevelResponseDto>>> GetLevelById(int id)
        {
            try
            {
                var level = await _levelService.GetLevelByIdAsync(id);
                if (level == null)
                {
                    return NotFound(ApiResponse<LevelResponseDto>.ErrorResponse(404, "Level not found"));
                }
                return Ok(ApiResponse<LevelResponseDto>.SuccessResponse(level));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<LevelResponseDto>.ErrorResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
