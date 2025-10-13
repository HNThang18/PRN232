using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public enum LogAction
    {
        Create,
        Update,
        Delete
    }
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        [Required]
        public LogAction Action { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; }

        public int EntityId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? Details { get; set; } // Có thể lưu dưới dạng JSON để chứa dữ liệu cũ và mới

        public virtual User User { get; set; }
    }
}
