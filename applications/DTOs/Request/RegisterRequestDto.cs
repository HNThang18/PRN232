using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs.Request
{
    public class RegisterRequestDto
    {
        [Required] public string Username { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required][MinLength(6)] public string Password { get; set; }

        // Cập nhật Regex để cho phép cả 'Admin' (nếu cần)
        [Required]
        [RegularExpression("^(Teacher|Student|Admin)$")]
        public string Role { get; set; }

        // === THÊM DÒNG NÀY ===
        [Required]
        public int? LevelId { get; set; }

        [MaxLength(50)]
        public string? GradeLevel { get; set; }
    }
}
