using applications.DTOs.Question;
using applications.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using services.Interfaces;

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
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QuestionResponseDto>>>> GetAll()
        {
            try
            {
                var questions = await _questionService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<QuestionResponseDto>>.SuccessResponse(questions));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<QuestionResponseDto>>.ErrorResponse(
                    500, $"Error retrieving questions: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<ApiResponse<QuestionResponseDto>>> GetById(int id)
        {
            try
            {
                var question = await _questionService.GetByIdAsync(id);
                if (question == null)
                {
                    return NotFound(ApiResponse<QuestionResponseDto>.ErrorResponse(404, "Question not found"));
                }

                return Ok(ApiResponse<QuestionResponseDto>.SuccessResponse(question));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QuestionResponseDto>.ErrorResponse(
                    500, $"Error retrieving question: {ex.Message}"));
            }
        }

        [HttpGet("available")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QuestionResponseDto>>>> GetAvailableQuestions(
            [FromQuery] int? levelId = null,
            [FromQuery] int? difficultyId = null,
            [FromQuery] string? topic = null,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var questions = await _questionService.GetAvailableQuestionsAsync(levelId, difficultyId, topic, searchTerm);
                return Ok(ApiResponse<IEnumerable<QuestionResponseDto>>.SuccessResponse(questions));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<QuestionResponseDto>>.ErrorResponse(
                    500, $"Error retrieving available questions: {ex.Message}"));
            }
        }

        [HttpGet("quiz/{quizId}")]
        [Authorize(Roles = "Teacher,Admin,Student")]
        public async Task<ActionResult<ApiResponse<IEnumerable<QuestionResponseDto>>>> GetByQuizId(int quizId)
        {
            try
            {
                var questions = await _questionService.GetByQuizIdAsync(quizId);
                return Ok(ApiResponse<IEnumerable<QuestionResponseDto>>.SuccessResponse(questions));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<QuestionResponseDto>>.ErrorResponse(
                    500, $"Error retrieving questions: {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<ApiResponse<QuestionResponseDto>>> Create([FromBody] CreateQuestionRequestDto request)
        {
            try
            {
                var question = await _questionService.CreateAsync(request);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = question.QuestionId },
                    ApiResponse<QuestionResponseDto>.SuccessResponse(question));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<QuestionResponseDto>.ErrorResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<QuestionResponseDto>.ErrorResponse(
                    500, $"Error creating question: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] UpdateQuestionRequestDto request)
        {
            try
            {
                var success = await _questionService.UpdateAsync(id, request);
                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse(404, "Question not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Question updated successfully" }));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    500, $"Error updating question: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var success = await _questionService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse(404, "Question not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Question deleted successfully" }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    500, $"Error deleting question: {ex.Message}"));
            }
        }
    }
}
