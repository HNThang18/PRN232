using Microsoft.AspNetCore.Mvc;
using repositories.Models;
using services.Interfaces;
using applications.DTOs.Difficulty;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DifficultyController : ControllerBase
    {
        private readonly IDifficultyService _difficultyService;

        public DifficultyController(IDifficultyService difficultyService)
        {
            _difficultyService = difficultyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _difficultyService.GetAllDifficultiesAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _difficultyService.GetDifficultyByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DifficultyCreateDto dto)
        {
            if (dto == null) return BadRequest();

            var difficulty = new Difficulty
            {
                Name = dto.Name,
                Description = dto.Description,
                Value = dto.Value
            };

            await _difficultyService.AddDifficultyAsync(difficulty);
            return CreatedAtAction(nameof(GetById), new { id = difficulty.DifficultyId }, difficulty);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DifficultyUpdateDto dto)
        {
            if (dto == null) return BadRequest();

            var existing = await _difficultyService.GetDifficultyByIdAsync(dto.DifficultyId);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Value = dto.Value;

            await _difficultyService.UpdateDifficultyAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _difficultyService.GetDifficultyByIdAsync(id);
            if (existing == null) return NotFound();
            await _difficultyService.DeleteDifficultyAsync(id);
            return NoContent();
        }
    }
}
