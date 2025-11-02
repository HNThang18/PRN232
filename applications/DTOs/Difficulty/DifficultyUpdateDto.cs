using System.ComponentModel.DataAnnotations;

namespace applications.DTOs.Difficulty
{
    public class DifficultyUpdateDto
    {
        [Required]
        public int DifficultyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public int Value { get; set; }
    }
}
