using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public class QuestionBank
    {
        [Key]
        public int QuestionBankId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int TeacherId { get; set; }

        [Required]
        [ForeignKey("Level")]
        public int LevelId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsPublic { get; set; } = false;

        public virtual User Teacher { get; set; }
        public virtual Level Level { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
