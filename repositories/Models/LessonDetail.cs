using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

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
    public class LessonDetail
    {
        [Key]
        public int LessonDetailId { get; set; }

        [Required]
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }

        public int Order { get; set; }

        [Required]
        [MaxLength(50)]
        public ContentType ContentType { get; set; }

        [Required]
        public string Content { get; set; } // Có thể là text, URL, ...
        public string? ContentLaTeX { get; set; } // Lưu nội dung dưới dạng LaTeX để render công thức

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual Lesson Lesson { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}
