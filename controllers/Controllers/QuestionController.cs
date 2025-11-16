using applications.DTOs.Question;
using applications.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using services.Interfaces;

namespace controllers.Controllers
{
    [ApiController]
    [Route("api/questions")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        /// <summary>
        /// Get available questions for adding to quiz (Approved status, not assigned to any quiz)
        /// </summary>
        /// <param name="levelId">Filter by level ID</param>
        /// <param name="difficultyId">Filter by difficulty ID</param>
        /// <param name="topic">Filter by topic (enum name)</param>
        /// <param name="searchTerm">Search in question text</param>
        /// <returns>List of available questions</returns>
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

                var questionDtos = questions.Select(q => new QuestionResponseDto
                {
                    QuestionId = q.QuestionId,
                    Topic = q.Topic.ToString(),
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType.ToString(),
                    DifficultyId = q.DifficultyId,
                    DifficultyName = q.Difficulty?.Name ?? "Unknown",
                    Status = q.Status.ToString(),
                    CorrectAnswer = q.CorrectAnswer,
                    Explanation = q.Explanation,
                    Tags = q.Tags,
                    IsAIGenerated = q.IsAIGenerated,
                    Answers = q.Answers?.Select(a => new AnswerResponseDto
                    {
                        AnswerId = a.AnswerId,
                        AnswerText = a.AnswerText,
                        IsCorrect = a.IsCorrect
                    }).ToList() ?? new List<AnswerResponseDto>()
                }).ToList();

                return Ok(ApiResponse<IEnumerable<QuestionResponseDto>>.SuccessResponse(questionDtos));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<QuestionResponseDto>>.ErrorResponse(
                    500, $"Error retrieving available questions: {ex.Message}"));
            }
        }
    }
}
