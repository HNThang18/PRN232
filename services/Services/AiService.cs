using applications.DTOs.Request.AI;
using applications.DTOs.Response.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace services.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiService> _logger;
        private readonly string _aiServiceBaseUrl;

        public AiService(HttpClient httpClient, IConfiguration configuration, ILogger<AiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _aiServiceBaseUrl = configuration["AiService:BaseUrl"] ?? "http://localhost:8000";
            _httpClient.BaseAddress = new Uri(_aiServiceBaseUrl);
            // Timeout is configured in Program.cs via AddHttpClient (5 minutes)
        }

        public async Task<AiLessonPlanResponseDto> GenerateLessonPlanAsync(
            AiLessonPlanRequestDto request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Calling AI service to generate lesson plan for topic: {Topic}", request.Topic);

                var aiRequest = new
                {
                    topic = request.Topic,
                    grade_level = request.GradeLevel,
                    duration = request.Duration,
                    objectives = request.Objectives,
                    additional_requirements = request.AdditionalRequirements
                };

                _logger.LogInformation("AI Request Body: {Request}", System.Text.Json.JsonSerializer.Serialize(aiRequest));

                var response = await _httpClient.PostAsJsonAsync(
                    "/api/v1/generate/lesson-plan", 
                    aiRequest, 
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                // Read raw response for debugging
                var rawContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Raw AI response: {Response}", rawContent);

                // Deserialize with case-insensitive options
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<AiApiResponse<AiLessonPlanResponseDto>>(rawContent, options);

                if (result == null || !result.Success || result.Data == null)
                {
                    _logger.LogError("AI service returned invalid response. Result: {Result}", rawContent);
                    throw new Exception($"AI service returned invalid response: {rawContent}");
                }

                _logger.LogInformation("Successfully generated lesson plan for topic: {Topic}", request.Topic);
                return result.Data;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to connect to AI service");
                throw new Exception("Failed to connect to AI service. Please ensure the AI service is running.", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize AI response");
                throw new Exception($"Failed to deserialize AI response: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating lesson plan");
                throw new Exception($"Error generating lesson plan: {ex.Message}", ex);
            }
        }

        public async Task<AiQuestionResponseDto> GenerateQuestionsAsync(
            AiQuestionRequestDto request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Calling AI service to generate {Count} questions for topic: {Topic}", 
                    request.Count, request.Topic);

                var aiRequest = new
                {
                    topic = request.Topic,
                    grade_level = request.GradeLevel.ToString(),
                    question_type = request.QuestionType,
                    difficulty = request.Difficulty,
                    count = request.Count,
                    include_solution = request.IncludeSolution
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "/api/v1/generate/questions", 
                    aiRequest, 
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                // Read raw response for debugging
                var rawContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Raw AI response: {Response}", rawContent);

                // Deserialize with case-insensitive options
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<AiApiResponse<AiQuestionResponseDto>>(rawContent, options);

                if (result == null || !result.Success || result.Data == null)
                {
                    _logger.LogError("AI service returned invalid response. Result: {Result}", rawContent);
                    throw new Exception($"AI service returned invalid response: {rawContent}");
                }

                _logger.LogInformation("Successfully generated {Count} questions for topic: {Topic}", 
                    request.Count, request.Topic);
                return result.Data;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to connect to AI service");
                throw new Exception("Failed to connect to AI service. Please ensure the AI service is running.", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize AI response");
                throw new Exception($"Failed to deserialize AI response: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating questions");
                throw new Exception($"Error generating questions: {ex.Message}", ex);
            }
        }

        public async Task<AiQuizResponseDto> GenerateQuizAsync(
            AiQuizRequestDto request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Calling AI service to generate quiz: {Title}", request.Title);

                var aiRequest = new
                {
                    title = request.Title,
                    topic = request.Topic,
                    grade_level = request.GradeLevel.ToString(),
                    duration = request.Duration,
                    question_count = request.QuestionCount,
                    difficulty_distribution = request.DifficultyDistribution,
                    include_essay = request.IncludeEssay
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "/api/v1/generate/quiz", 
                    aiRequest, 
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                // Read raw response for debugging
                var rawContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation("Raw AI response: {Response}", rawContent);

                // Deserialize with case-insensitive options
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<AiApiResponse<AiQuizResponseDto>>(rawContent, options);

                if (result == null || !result.Success || result.Data == null)
                {
                    _logger.LogError("AI service returned invalid response. Result: {Result}", rawContent);
                    throw new Exception($"AI service returned invalid response: {rawContent}");
                }

                _logger.LogInformation("Successfully generated quiz: {Title}", request.Title);
                return result.Data;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to connect to AI service");
                throw new Exception("Failed to connect to AI service. Please ensure the AI service is running.", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize AI response");
                throw new Exception($"Failed to deserialize AI response: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating quiz");
                throw new Exception($"Error generating quiz: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("/health", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Helper class for deserializing AI API responses
        private class AiApiResponse<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public AiErrorDetail? Error { get; set; }
        }

        private class AiErrorDetail
        {
            public int Code { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        public async Task<AiChatResponseDto> ChatAsync(AiChatRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Calling AI service for chat: {Message}", request.Message);

                var aiRequest = new
                {
                    message = request.Message,
                    conversation_id = request.ConversationId,
                    user_role = request.UserRole ?? "user"
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "/api/v1/chat",
                    aiRequest,
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var rawContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogDebug("AI chat response: {Response}", rawContent);

                var chatResponse = JsonSerializer.Deserialize<AiChatResponseDto>(
                    rawContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (chatResponse == null)
                {
                    throw new Exception("Failed to deserialize chat response from AI service");
                }

                return chatResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling AI chat service");
                throw new Exception($"Failed to connect to AI service: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize AI chat response");
                throw new Exception("Invalid response format from AI service", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AI chat");
                throw;
            }
        }
    }
}
