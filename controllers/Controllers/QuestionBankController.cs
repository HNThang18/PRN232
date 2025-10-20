using Microsoft.AspNetCore.Mvc;
using repositories.Models;
using services.Interfaces;
using applications.DTOs.QuestionBank;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionBankController : ControllerBase
    {

        private readonly IQuestionBankService _questionBankService;

        public QuestionBankController(IQuestionBankService questionBankService)
        {
            _questionBankService = questionBankService;
        }

        private static QuestionBankDto MapToDto(QuestionBank qb)
        {
            return new QuestionBankDto
            {
                QuestionBankId = qb.QuestionBankId,
                TeacherId = qb.TeacherId,
                TeacherName = qb.Teacher?.Username,
                LevelId = qb.LevelId,
                LevelName = qb.Level?.LevelName,
                Name = qb.Name,
                Description = qb.Description,
                IsPublic = qb.IsPublic
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _questionBankService.GetAllQuestionBanksAsync();
            var dtos = items.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _questionBankService.GetQuestionBankByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(MapToDto(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] QuestionBankCreateDto dto)
        {
            if (dto == null) return BadRequest();

            var questionBank = new QuestionBank
            {
                TeacherId = dto.TeacherId,
                LevelId = dto.LevelId,
                Name = dto.Name,
                Description = dto.Description,
                IsPublic = dto.IsPublic
            };

            await _questionBankService.AddQuestionBankAsync(questionBank);
            var resultDto = MapToDto(questionBank);
            return CreatedAtAction(nameof(GetById), new { id = resultDto.QuestionBankId }, resultDto);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] QuestionBankUpdateDto dto)
        {
            if (dto == null) return BadRequest();

            var existing = await _questionBankService.GetQuestionBankByIdAsync(dto.QuestionBankId);
            if (existing == null) return NotFound();

            existing.TeacherId = dto.TeacherId;
            existing.LevelId = dto.LevelId;
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.IsPublic = dto.IsPublic;

            await _questionBankService.UpdateQuestionBankAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _questionBankService.GetQuestionBankByIdAsync(id);
            if (existing == null) return NotFound();
            await _questionBankService.DeleteQuestionBankAsync(id);
            return NoContent();
        }
    }
}
