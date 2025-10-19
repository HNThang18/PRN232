using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public class Attachment
    {
        [Key]
        public int AttachmentId { get; set; }

        [Required]
        [ForeignKey("LessonDetail")]    
        public int LessonDetailId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } // Tên file hiển thị

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } // URL hoặc đường dẫn lưu file


        [MaxLength(50)]
        public string FileType { get; set; } // "image/png", "application/pdf", ...
        public long FileSize { get; set; } // Kích thước file tính bằng bytes

        public int? UploadedBy { get; set; } // FK → User (nếu có quản lý người upload)
        public DateTime UploadTimestamp { get; set; }

        // Navigation
        public virtual LessonDetail LessonDetail { get; set; }
        public virtual User? User { get; set; }
    }
}
