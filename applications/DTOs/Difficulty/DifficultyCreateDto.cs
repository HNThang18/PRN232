using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.Difficulty
{
    public class DifficultyCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Value { get; set; }
    }
}
