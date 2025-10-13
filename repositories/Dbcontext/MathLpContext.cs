using Microsoft.EntityFrameworkCore;
using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repositories.Dbcontext
{
    public class MathLpContext : DbContext
    {
        public MathLpContext() { }
        public MathLpContext(DbContextOptions<MathLpContext> options) : base(options)
        {
        }
        public DbSet<AIRequest> aIRequests { get; set; }
        public DbSet<AuditLog> auditLogs { get; set; }
        public DbSet<Difficulty> difficulties { get; set; }
        public DbSet<Lesson> lessons { get; set; }
        public DbSet<LessonDetail> lessonDetails { get; set; }
        public DbSet<LessonPlan> lessonPlans { get; set; }
        public DbSet<Level> levels { get; set; }
        public DbSet<Progress> progresses { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Answer> answers { get; set; }
        public DbSet<Question> questions { get; set; }
        public DbSet<QuestionBank> questionBanks { get; set; }
        public DbSet<Quiz> quizzes { get; set; }
        public DbSet<Submission> submissions { get; set; }
        public DbSet<SubmissionDetail> submissionDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<AuditLog>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<AIRequest>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Difficulty>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Lesson>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<LessonDetail>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<LessonPlan>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Level>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Progress>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<User>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Answer>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Question>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<QuestionBank>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Quiz>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Submission>().Property(e => e.Id).ValueGeneratedOnAdd();
            //modelBuilder.Entity<SubmissionDetail>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<AIRequest>()
                .HasOne(ar => ar.User)
                .WithMany(u => u.AIRequests)
                .HasForeignKey(ar => ar.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LessonPlan>()
                .HasOne(lp => lp.Teacher)
                .WithMany(u => u.LessonPlans)
                .HasForeignKey(lp => lp.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Teacher)
                .WithMany(u => u.Quizzes)
                .HasForeignKey(q => q.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LessonPlan>()
                .HasOne(lp => lp.AIRequest)
                .WithMany(ar => ar.LessonPlans)
                .HasForeignKey(lp => lp.AIRequestId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.AIRequest)
                .WithMany(ar => ar.Questions)
                .HasForeignKey(q => q.AIRequestId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Quiz)
                .WithMany(q => q.Submissions)
                .HasForeignKey(s => s.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubmissionDetail>()
                .HasOne(sd => sd.Question)
                .WithMany(q => q.SubmissionDetails)
                .HasForeignKey(sd => sd.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubmissionDetail>()
                .HasOne(sd => sd.Submission)
                .WithMany(s => s.SubmissionDetails)
                .HasForeignKey(sd => sd.SubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuestionBank>()
                .HasOne(qb => qb.Teacher)
                .WithMany(u => u.QuestionBanks)
                .HasForeignKey(qb => qb.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LessonPlan>()
                .HasOne(lp => lp.Level)
                .WithMany(l => l.LessonPlans)
                .HasForeignKey(lp => lp.LevelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
