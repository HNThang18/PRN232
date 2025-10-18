using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applications.DTOs
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // Trả về string (Student, Teacher, Admin)
        public bool IsActive { get; set; }
        public int? LevelId { get; set; }
        public string? LevelName { get; set; } // Chúng ta sẽ join để lấy tên
        public string? GradeLevel { get; set; }
        public decimal Credit { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
