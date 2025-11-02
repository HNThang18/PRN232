using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace repositories.Models
{
    public enum ContentType
    {
        Text,
        Video,
        Image,
        Audio,
        Quiz
    }

    [Index(nameof(LessonId), nameof(Order), IsUnique = true, Name = "IX_LessonDetail_LessonId_Order")]
    [Index(nameof(ContentType))]
    public class LessonDetail
    {
        [Key]
        public int LessonDetailId { get; set; }

        [Required]
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Order phải lớn hơn 0")]
        public int Order { get; set; }  // Thứ tự hiển thị trong Lesson

        [Required]
        public ContentType ContentType { get; set; }  // Loại nội dung (Text, Video,...)

        [Required]
        [StringLength(10000, ErrorMessage = "Content không được vượt quá 10000 ký tự")]
        public string Content { get; set; } // Có thể là text, URL, ...

        [StringLength(10000, ErrorMessage = "ContentLaTeX không được vượt quá 10000 ký tự")]
        public string? ContentLaTeX { get; set; } // Lưu nội dung dưới dạng LaTeX để render công thức

        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual Lesson Lesson { get; set; } = null!;
        public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
