using Microsoft.AspNetCore.Mvc;
using applications.DTOs.Response;
using repositories.Models;
using services.Interfaces;
using applications.DTOs.Request;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _questionService.GetAllAsync();
            return Ok(ApiResponse<List<Question>>.SuccessResponse(list));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _questionService.GetByIdAsync(id);
            if (item == null) return NotFound(ApiResponse<Question>.ErrorResponse(404, "Question not found"));
            return Ok(ApiResponse<Question>.SuccessResponse(item));
        }

        [HttpGet("quiz/{quizId:int}")]
        public async Task<IActionResult> GetByQuiz(int quizId)
        {
            var list = await _questionService.GetByQuizIdAsync(quizId);
            return Ok(ApiResponse<List<Question>>.SuccessResponse(list));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] QuestionRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Question>.ErrorResponse(400, "Invalid request"));

            var question = new Question
            {
                QuizId = dto.QuizId,
                QuestionBankId = dto.QuestionBankId,
                DifficultyId = dto.DifficultyId,
                Topic = (Topic)dto.Topic,
                QuestionText = dto.QuestionText,
                QuestionType = (QuestionType)dto.QuestionType,
                CorrectAnswer = dto.CorrectAnswer,
                Explanation = dto.Explanation,
                Tags = dto.Tags,
                IsAIGenerated = dto.IsAIGenerated,
                Status = (QuestionStatus)dto.Status
            };

            var created = await _questionService.CreateAsync(question);
            return Ok(ApiResponse<Question>.SuccessResponse(created));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] QuestionRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Question>.ErrorResponse(400, "Invalid request"));

            var question = new Question
            {
                QuestionId = id,
                QuizId = dto.QuizId,
                QuestionBankId = dto.QuestionBankId,
                DifficultyId = dto.DifficultyId,
                Topic = (Topic)dto.Topic,
                QuestionText = dto.QuestionText,
                QuestionType = (QuestionType)dto.QuestionType,
                CorrectAnswer = dto.CorrectAnswer,
                Explanation = dto.Explanation,
                Tags = dto.Tags,
                IsAIGenerated = dto.IsAIGenerated,
                Status = (QuestionStatus)dto.Status
            };

            var updated = await _questionService.UpdateAsync(id, question);
            if (!updated) return NotFound(ApiResponse<Question>.ErrorResponse(404, "Question not found"));
            return Ok(ApiResponse<Question>.SuccessResponse(question));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _questionService.DeleteAsync(id);
            if (!deleted) return NotFound(ApiResponse<Question>.ErrorResponse(404, "Question not found"));
            return Ok(ApiResponse<string>.SuccessResponse("Deleted"));
        }
    }
}
