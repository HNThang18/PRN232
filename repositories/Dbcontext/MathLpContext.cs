using Microsoft.EntityFrameworkCore;
using repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using applications.Interfaces;
namespace repositories.Dbcontext
{
    public class MathLpContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        //public MathLpContext() { }
        public MathLpContext(DbContextOptions<MathLpContext> options, ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }
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
        public DbSet<Attachment> attachments { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Lấy danh sách các thay đổi
            var auditEntries = OnBeforeSaveChanges();

            // Lưu các thay đổi (bao gồm cả dữ liệu gốc VÀ các audit log mới)
            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

            var currentUserId = _currentUserService.GetUserId();
            var currentIpAddress = _currentUserService.GetIpAddress();

            foreach (var entry in ChangeTracker.Entries())
            {
                // Bỏ qua các entity không cần log hoặc chính entity AuditLog
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry
                {
                    TableName = entry.Metadata.GetTableName() ?? "Unknown",
                    UserId = currentUserId,
                    IpAddress = currentIpAddress,
                    Timestamp = DateTime.UtcNow
                };

                var serializerOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                    WriteIndented = false
                };

                string? details = null;

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.Action = LogAction.Create; //
                        details = JsonSerializer.Serialize(entry.CurrentValues.ToObject(), serializerOptions);
                        auditEntry.EntityId = GetPrimaryKey(entry);
                        break;

                    case EntityState.Deleted:
                        auditEntry.Action = LogAction.Delete; //
                        details = JsonSerializer.Serialize(entry.OriginalValues.ToObject(), serializerOptions);
                        auditEntry.EntityId = GetPrimaryKey(entry);
                        break;

                    case EntityState.Modified:
                        if (entry.Properties.Any(p => p.IsModified))
                        {
                            auditEntry.Action = LogAction.Update; //
                            var changes = new
                            {
                                Old = entry.OriginalValues.ToObject(),
                                New = entry.CurrentValues.ToObject()
                            };
                            details = JsonSerializer.Serialize(changes, serializerOptions);
                            auditEntry.EntityId = GetPrimaryKey(entry);
                        }
                        break;
                }

                if (details != null)
                {
                    auditEntry.Details = details;
                    auditEntries.Add(auditEntry);
                }
            }

            // Thêm tất cả các entry log vào DbContext
            foreach (var auditEntry in auditEntries)
            {
                auditLogs.Add(auditEntry.ToAuditLog());
            }

            return auditEntries;
        }

        // Lớp helper tạm thời
        public class AuditEntry
        {
            public string TableName { get; set; }
            public int? UserId { get; set; }
            public LogAction Action { get; set; } //
            public int? EntityId { get; set; }
            public DateTime Timestamp { get; set; }
            public string? Details { get; set; }
            public string? IpAddress { get; set; }

            public AuditLog ToAuditLog()
            {
                return new AuditLog
                {
                    UserId = this.UserId,
                    Action = this.Action,
                    EntityName = this.TableName,
                    EntityId = this.EntityId,
                    Timestamp = this.Timestamp,
                    Details = this.Details ?? "{}",
                    IpAddress = this.IpAddress
                };
            }
        }

        private int? GetPrimaryKey(EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey()?.Properties.FirstOrDefault();
            if (key != null)
            {
                var value = entry.Property(key.Name).CurrentValue;
                if (value != null && value is int)
                {
                    return (int)value;
                }
            }
            return null;
        }

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

            modelBuilder.Entity<AiRequest>()
                .HasOne(ar => ar.User)
                .WithMany(u => u.AiRequests)
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
                .HasOne(lp => lp.AiRequest)
                .WithMany(ar => ar.LessonPlans)
                .HasForeignKey(lp => lp.AiRequestId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.AiRequest)
                .WithMany(ar => ar.Questions)
                .HasForeignKey(q => q.AiRequestId)
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

            // Specify decimal precision for User.Credit
            modelBuilder.Entity<User>()
                .Property(u => u.Credit)
                .HasColumnType("decimal(18,2)");

            // Sample data seeding
            modelBuilder.Entity<Level>().HasData(
                new Level { LevelId = 1, LevelName = "Primary School", EducationLevel = EducationLevel.PrimarySchool, Order = 1 },
                new Level { LevelId = 2, LevelName = "Secondary School", EducationLevel = EducationLevel.SecondarySchool, Order = 2 },
                new Level { LevelId = 3, LevelName = "High School", EducationLevel = EducationLevel.HighSchool, Order = 3 }
            );

            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, LevelId = 1, Username = "student1", Email = "student1@example.com", Password = "hashedpassword", Credit = 0, Role = UserRole.Student, IsActive = true, CreatedAt = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc) },
                new User { UserId = 2, LevelId = 2, Username = "teacher1", Email = "teacher1@example.com", Password = "hashedpassword", Credit = 100, Role = UserRole.Teacher, IsActive = true, CreatedAt = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<Difficulty>().HasData(
                new Difficulty { DifficultyId = 1, Name = "Easy", Description = "Basic level", Value = 1 },
                new Difficulty { DifficultyId = 2, Name = "Medium", Description = "Intermediate level", Value = 2 },
                new Difficulty { DifficultyId = 3, Name = "Hard", Description = "Advanced level", Value = 3 }
            );

            modelBuilder.Entity<AiRequest>().HasData(
                new AiRequest { AIRequestId = 1, UserId = 2, LevelId = 2, RequestType = RequestType.GenerateLessonPlan, Prompt = "Generate a lesson plan for algebra.", Response = "Lesson plan generated.", Status = AiRequestStatus.Success, Cost = 1.5M, CreatedAt = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<LessonPlan>().HasData(
                new LessonPlan { LessonPlanId = 1, TeacherId = 2, LevelId = 2, AiRequestId = 1, Title = "Algebra Basics", Duration = 60, Content = "Introduction to Algebra", Status = LessonPlanStatus.Published, Version = 1, CreatedAt = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<Lesson>().HasData(
                new Lesson { LessonId = 1, LessonPlanId = 1, Title = "What is Algebra?", Content = "Algebra is ...", PublishedDate = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc), IsShared = true }
            );

            modelBuilder.Entity<LessonDetail>().HasData(
                new LessonDetail { LessonDetailId = 1, LessonId = 1, Order = 1, ContentType = ContentType.Text, Content = "Algebra is a branch of mathematics ...", ContentLaTeX = null, CreatedAt = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<Attachment>().HasData(
                new Attachment { AttachmentId = 1, LessonDetailId = 1, FileName = "intro.pdf", FilePath = "/files/intro.pdf", FileSize = 102400, FileType = "pdf", UploadTimestamp = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<QuestionBank>().HasData(
                new QuestionBank { QuestionBankId = 1, TeacherId = 2, LevelId = 2, Name = "Algebra Questions", Description = "Basic algebra questions", IsPublic = true }
            );

            modelBuilder.Entity<Quiz>().HasData(
                new Quiz { QuizId = 1, TeacherId = 2, LevelId = 2, Title = "Algebra Quiz", TimeLimit = 30, AttemptLimit = 3, TotalScore = 100, IsAIGenerated = false, Status = QuizStatus.Published, CreatedAt = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<Question>().HasData(
                new Question { QuestionId = 1, QuizId = 1, QuestionBankId = 1, DifficultyId = 1, AiRequestId = 1, Topic = Topic.Algebra, QuestionText = "What is x in 2x=4?", QuestionType = QuestionType.MultipleChoice, CorrectAnswer = "2", Explanation = "Divide both sides by 2.", Tags = "algebra,equation", IsAIGenerated = false, Status = QuestionStatus.Approved, CreatedAt = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<Answer>().HasData(
                new Answer { AnswerId = 1, QuestionId = 1, AnswerText = "2", IsCorrect = true },
                new Answer { AnswerId = 2, QuestionId = 1, AnswerText = "4", IsCorrect = false }
            );

            modelBuilder.Entity<Submission>().HasData(
                new Submission { SubmissionId = 1, StudentId = 1, QuizId = 1, SubmittedAt = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc), Status = SubissionStatus.Completed, Score = 100, Feedback = "Great job!", AttemptNumber = 1, DurationTaken = 1200 }
            );

            modelBuilder.Entity<SubmissionDetail>().HasData(
                new SubmissionDetail { SubmissionDetailId = 1, SubmissionId = 1, QuestionId = 1, StudentAnswer = "2", IsCorrect = true, ScoreEarned = 100, Explanation = "Correct answer." }
            );

            modelBuilder.Entity<Progress>().HasData(
                new Progress { ProgressId = 1, StudentId = 1, LessonId = 1, CompletionStatus = ProgressStatus.Completed, AttemptDate = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc), IsActive = true }
            );

            modelBuilder.Entity<AuditLog>().HasData(
                new AuditLog { LogId = 1, UserId = 2, Action = LogAction.Create, EntityName = "LessonPlan", EntityId = 1, Timestamp = new DateTime(2024, 6, 14, 8, 51, 0, DateTimeKind.Utc), Details = "Created lesson plan." }
            );
        }
    }
}
