using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Models
{
    public class SubmissionDetail
    {
        [Key]
        public int SubmissionDetailId { get; set; }

        [Required]
        [ForeignKey("Submission")]
        public int SubmissionId { get; set; }

        [Required]
        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        public string StudentAnswer { get; set; }

        public bool IsCorrect { get; set; }

        public decimal ScoreEarned { get; set; }

        public string? Explanation { get; set; }

        public virtual Submission Submission { get; set; }
        public virtual Question Question { get; set; }
    }
}
