using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.QuestionBank
{
    public class QuestionBankCreateDto
    {
        [Required]
        public int TeacherId { get; set; }

        [Required]
        public int LevelId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsPublic { get; set; } = false;
    }
}
