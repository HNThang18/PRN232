using Azure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public enum LessonPlanStatus
    {
        Draft,
        Published,
        Deleted
    }
    public class LessonPlan
    {
        [Key]
        public int LessonPlanId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int TeacherId { get; set; }

        [Required]
        [ForeignKey("Level")]
        public int LevelId { get; set; } // Cấp học (Tiểu học / THCS / THPT)

        [Required]
        public int Grade { get; set; } // Lớp cụ thể (1–12)

        [ForeignKey("AiRequest")]
        public int? AiRequestId { get; set; }


        [Required]
        [MaxLength(200)]
        public string Title { get; set; } // Tên giáo án

        [MaxLength(200)]
        public string Topic { get; set; } // Chủ đề toán học (ví dụ: Phân số, Hàm số)

        public string? LearningObjectives { get; set; } // Mục tiêu bài học
        public string? MathFormulas { get; set; } // Công thức Toán (dạng LaTeX)

        public string? AiPrompt { get; set; } // Prompt gửi AI
        public string? AiResponse { get; set; } // Kết quả trả về từ AI (JSON hoặc text)

        public int Duration { get; set; } // Thời lượng tính bằng phút

        public string? Content { get; set; } // Nội dung mô tả tổng thể

        [Required]
        public LessonPlanStatus Status { get; set; } = LessonPlanStatus.Draft;

        public int Version { get; set; } = 1;
        public bool IsPublic { get; set; } = false; // Cho phép chia sẻ

        public string? Tags { get; set; }

        public string? ExportPath { get; set; }  // File Word hoặc PDF

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        // Navigation properties
        public virtual User Teacher { get; set; }
        public virtual Level Level { get; set; }
        public virtual AiRequest AiRequest { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
