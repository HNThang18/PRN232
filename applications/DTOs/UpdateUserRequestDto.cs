using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs
{
    public class UpdateUserRequestDto
    {
        [EmailAddress]
        public string? Email { get; set; }

        [RegularExpression("^(Teacher|Student|Admin)$")]
        public string? Role { get; set; }

        public bool? IsActive { get; set; }

        public int? LevelId { get; set; }

        [MaxLength(50)]
        public string? GradeLevel { get; set; }

        public decimal? Credit { get; set; }

        // Admin cũng có thể reset mật khẩu
        [MinLength(6)]
        public string? NewPassword { get; set; }
    }
}
