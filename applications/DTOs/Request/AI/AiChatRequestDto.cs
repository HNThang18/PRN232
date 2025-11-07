using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.Request.AI
{
    public class AiChatRequestDto
    {
        [Required(ErrorMessage = "Message is required")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 5000 characters")]
        public string Message { get; set; } = string.Empty;

        public string? ConversationId { get; set; }

        public string? UserRole { get; set; } = "user";
    }

    public class AiChatMessage
    {
        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime? Timestamp { get; set; }
    }
}
