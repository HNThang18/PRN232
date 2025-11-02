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
    public enum Topic
    {
        Calculus,
        Trigonometry,
        Geometry,
        Algebra,
        Statistics,
        Probability,
        NumberTheory,
        Combinatorics,
        LinearAlgebra,
        DifferentialEquations
    }
    public enum QuestionType
    {
        MultipleChoice,
        FillInTheBlank,
        TrueFalse,
        ShortAnswer,
        Essay
    }
    public enum QuestionStatus
    {
        PendingReview,
        Approved,
        Rejected,
        NeedsRevision
    }
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [ForeignKey("Quiz")]
        public int? QuizId { get; set; }

        [ForeignKey("QuestionBank")]
        public int? QuestionBankId { get; set; }

        [ForeignKey("Difficulty")]
        public int? DifficultyId { get; set; }

        [ForeignKey("AiRequest")]
        public int? AiRequestId { get; set; }

        [Required]
        public Topic Topic { get; set; }
        [Required]
        public string QuestionText { get; set; }

        [Required]
        public QuestionType QuestionType { get; set; }

        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public string? Tags { get; set; }

        public bool IsAIGenerated { get; set; }

        [Required]
        public QuestionStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual Quiz Quiz { get; set; }
        public virtual QuestionBank QuestionBank { get; set; }
        public virtual Difficulty Difficulty { get; set; }
        public virtual AiRequest AiRequest { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<SubmissionDetail> SubmissionDetails { get; set; }
    }
}
