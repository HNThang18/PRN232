using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace repositories.Models
{
    [Index(nameof(LessonPlanId))] // Index cho foreign key - tăng tốc query
    [Index(nameof(IsShared))]     // Index cho filter IsShared
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required(ErrorMessage = "LessonPlanId là bắt buộc")]
        [ForeignKey("LessonPlan")]
        public int LessonPlanId { get; set; }


        [Required(ErrorMessage = "Tiêu đề bài học là bắt buộc")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tiêu đề phải từ 3-200 ký tự")]
        [Column(TypeName = "nvarchar(200)")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        public string? Objective { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Content { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Example { get; set; }

        [StringLength(500, ErrorMessage = "URL không được vượt quá 500 ký tự")]
        [Url(ErrorMessage = "ResourceUrl phải là URL hợp lệ")]
        public string? ResourceUrl { get; set; }

        [Required(ErrorMessage = "Order là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Order phải lớn hơn 0")]
        public int Order { get; set; }

        public bool IsGeneratedByAI { get; set; } = false;
        public bool IsShared { get; set; } = false;

        public DateTime? PublishedDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(LessonPlanId))]
        public virtual LessonPlan LessonPlan { get; set; }

        // Chi tiết bài học (nhiều phần nhỏ trong Lesson)
        public virtual ICollection<LessonDetail> LessonDetails { get; set; } = new List<LessonDetail>();
        // Tiến trình học sinh (nếu có phần học sinh theo dõi tiến độ)
        public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    }
}
