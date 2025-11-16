using Microsoft.AspNetCore.Mvc;
using applications.DTOs.Response;
using repositories.Models;
using services.Interfaces;
using applications.DTOs.Request;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        public AnswersController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _answerService.GetAllAsync();
            return Ok(ApiResponse<List<Answer>>.SuccessResponse(list));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _answerService.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<Answer>.ErrorResponse(404, "Answer not found"));
            return Ok(ApiResponse<Answer>.SuccessResponse(item));
        }

        [HttpGet("question/{questionId:int}")]
        public async Task<IActionResult> GetByQuestion(int questionId)
        {
            var list = await _answerService.GetByQuestionIdAsync(questionId);
            return Ok(ApiResponse<List<Answer>>.SuccessResponse(list));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AnswerRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Answer>.ErrorResponse(400, "Invalid request"));

            var answer = new Answer
            {
                QuestionId = dto.QuestionId,
                AnswerText = dto.AnswerText,
                IsCorrect = dto.IsCorrect
            };

            var created = await _answerService.CreateAsync(answer);
            return Ok(ApiResponse<Answer>.SuccessResponse(created));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AnswerRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Answer>.ErrorResponse(400, "Invalid request"));

            var answer = new Answer
            {
                AnswerId = id,
                QuestionId = dto.QuestionId,
                AnswerText = dto.AnswerText,
                IsCorrect = dto.IsCorrect
            };

            var updated = await _answerService.UpdateAsync(id, answer);
            if (!updated) return NotFound(ApiResponse<Answer>.ErrorResponse(404, "Answer not found"));
            return Ok(ApiResponse<Answer>.SuccessResponse(answer));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _answerService.DeleteAsync(id);
            if (!deleted) return NotFound(ApiResponse<Answer>.ErrorResponse(404, "Answer not found"));
            return Ok(ApiResponse<string>.SuccessResponse("Deleted"));
        }
    }
}
