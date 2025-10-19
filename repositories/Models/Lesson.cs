using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required]
        [ForeignKey("LessonPlan")]
        public int LessonPlanId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } // Tên phần học (Ví dụ: "Khái niệm phân số")

        public string? Objective { get; set; } // Mục tiêu riêng của phần này
        public string? Content { get; set; } // Mô tả tổng quan nội dung
        public string? Example { get; set; } // Ví dụ minh họa chính
        public string? ResourceUrl { get; set; } // Ảnh / video / minh họa bổ sung

        public int Order { get; set; } // Thứ tự trong giáo án

        public bool IsGeneratedByAI { get; set; } = false;
        public bool IsShared { get; set; } = false;

        public DateTime? PublishedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // Navigation
        public virtual LessonPlan LessonPlan { get; set; }
        // Chi tiết bài học (nhiều phần nhỏ trong Lesson)
        public virtual ICollection<LessonDetail> LessonDetails { get; set; }
        // Tiến trình học sinh (nếu có phần học sinh theo dõi tiến độ)
        public virtual ICollection<Progress> Progresses { get; set; }
    }
}
