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
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        public long FileSize { get; set; } // Kích thước file tính bằng bytes

        [MaxLength(100)]
        public string FileType { get; set; }

        public DateTime UploadTimestamp { get; set; } = DateTime.UtcNow;

        public virtual LessonDetail LessonDetail { get; set; }
    }
}
