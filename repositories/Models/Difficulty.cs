using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public class Difficulty
    {
        [Key]
        public int DifficultyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } // Ví dụ: "Easy", "Medium", "Hard"

        public string? Description { get; set; }

        public int Value { get; set; } // Ví dụ: 1, 2, 3

        public virtual ICollection<Question> Questions { get; set; }
    }
}
