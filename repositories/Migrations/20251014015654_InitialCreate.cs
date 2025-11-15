using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "difficulties",
                columns: table => new
                {
                    DifficultyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_difficulties", x => x.DifficultyId);
                });

            migrationBuilder.CreateTable(
                name: "levels",
                columns: table => new
                {
                    LevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EducationLevel = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_levels", x => x.LevelId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    GradeLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_users_levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "levels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AiRequest",
                columns: table => new
                {
                    AIRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiRequest", x => x.AIRequestId);
                    table.ForeignKey(
                        name: "FK_AiRequest_levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "levels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AiRequest_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "auditLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<int>(type: "int", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_auditLogs_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "questionBanks",
                columns: table => new
                {
                    QuestionBankId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questionBanks", x => x.QuestionBankId);
                    table.ForeignKey(
                        name: "FK_questionBanks_levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "levels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_questionBanks_users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quizzes",
                columns: table => new
                {
                    QuizId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TimeLimit = table.Column<int>(type: "int", nullable: false),
                    AttemptLimit = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    IsAIGenerated = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quizzes", x => x.QuizId);
                    table.ForeignKey(
                        name: "FK_quizzes_levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "levels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quizzes_users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lessonPlans",
                columns: table => new
                {
                    LessonPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    AiRequestId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    ExportPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lessonPlans", x => x.LessonPlanId);
                    table.ForeignKey(
                        name: "FK_lessonPlans_AiRequest_AiRequestId",
                        column: x => x.AiRequestId,
                        principalTable: "AiRequest",
                        principalColumn: "AIRequestId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_lessonPlans_levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "levels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lessonPlans_users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: true),
                    QuestionBankId = table.Column<int>(type: "int", nullable: true),
                    DifficultyId = table.Column<int>(type: "int", nullable: true),
                    AiRequestId = table.Column<int>(type: "int", nullable: true),
                    Topic = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAIGenerated = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_questions_AiRequest_AiRequestId",
                        column: x => x.AiRequestId,
                        principalTable: "AiRequest",
                        principalColumn: "AIRequestId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_questions_difficulties_DifficultyId",
                        column: x => x.DifficultyId,
                        principalTable: "difficulties",
                        principalColumn: "DifficultyId");
                    table.ForeignKey(
                        name: "FK_questions_questionBanks_QuestionBankId",
                        column: x => x.QuestionBankId,
                        principalTable: "questionBanks",
                        principalColumn: "QuestionBankId");
                    table.ForeignKey(
                        name: "FK_questions_quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "quizzes",
                        principalColumn: "QuizId");
                });

            migrationBuilder.CreateTable(
                name: "submissions",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    DurationTaken = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submissions", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_submissions_quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submissions_users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lessons",
                columns: table => new
                {
                    LessonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonPlanId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsShared = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lessons", x => x.LessonId);
                    table.ForeignKey(
                        name: "FK_lessons_lessonPlans_LessonPlanId",
                        column: x => x.LessonPlanId,
                        principalTable: "lessonPlans",
                        principalColumn: "LessonPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "answers",
                columns: table => new
                {
                    AnswerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answers", x => x.AnswerId);
                    table.ForeignKey(
                        name: "FK_answers_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "submissionDetails",
                columns: table => new
                {
                    SubmissionDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    StudentAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    ScoreEarned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submissionDetails", x => x.SubmissionDetailId);
                    table.ForeignKey(
                        name: "FK_submissionDetails_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_submissionDetails_submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "submissions",
                        principalColumn: "SubmissionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lessonDetails",
                columns: table => new
                {
                    LessonDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentLaTeX = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lessonDetails", x => x.LessonDetailId);
                    table.ForeignKey(
                        name: "FK_lessonDetails_lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "lessons",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "progresses",
                columns: table => new
                {
                    ProgressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    CompletionStatus = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    AttemptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_progresses", x => x.ProgressId);
                    table.ForeignKey(
                        name: "FK_progresses_lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "lessons",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_progresses_users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attachments",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonDetailId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UploadTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attachments", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "FK_attachments_lessonDetails_LessonDetailId",
                        column: x => x.LessonDetailId,
                        principalTable: "lessonDetails",
                        principalColumn: "LessonDetailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "difficulties",
                columns: new[] { "DifficultyId", "Description", "Name", "Value" },
                values: new object[,]
                {
                    { 1, "Basic level", "Easy", 1 },
                    { 2, "Intermediate level", "Medium", 2 },
                    { 3, "Advanced level", "Hard", 3 }
                });

            migrationBuilder.InsertData(
                table: "levels",
                columns: new[] { "LevelId", "EducationLevel", "LevelName", "Order" },
                values: new object[,]
                {
                    { 1, 0, "Primary School", 1 },
                    { 2, 1, "Secondary School", 2 },
                    { 3, 2, "High School", 3 }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "UserId", "CreatedAt", "Credit", "Email", "GradeLevel", "IsActive", "LevelId", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), 0m, "student1@example.com", null, true, 1, "$2a$11$X5GWjenMG8dMT4rI9sIGjutxFe9l6xm0VBTHFuArHL.CDkfMursM6", 0, "student1" },
                    { 2, new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), 100m, "teacher1@example.com", null, true, 2, "$2a$11$X5GWjenMG8dMT4rI9sIGjutxFe9l6xm0VBTHFuArHL.CDkfMursM6", 1, "teacher1" }
                });

            migrationBuilder.InsertData(
                table: "AiRequest",
                columns: new[] { "AIRequestId", "Cost", "CreatedAt", "LevelId", "Prompt", "RequestType", "Response", "Status", "UserId" },
                values: new object[] { 1, 1.5m, new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), 2, "Generate a lesson plan for algebra.", 0, "Lesson plan generated.", 1, 2 });

            migrationBuilder.InsertData(
                table: "auditLogs",
                columns: new[] { "LogId", "Action", "Details", "EntityId", "EntityName", "Timestamp", "UserId" },
                values: new object[] { 1, 0, "Created lesson plan.", 1, "LessonPlan", new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.InsertData(
                table: "questionBanks",
                columns: new[] { "QuestionBankId", "Description", "IsPublic", "LevelId", "Name", "TeacherId" },
                values: new object[] { 1, "Basic algebra questions", true, 2, "Algebra Questions", 2 });

            migrationBuilder.InsertData(
                table: "quizzes",
                columns: new[] { "QuizId", "AttemptLimit", "CreatedAt", "IsAIGenerated", "LevelId", "PublishedAt", "Status", "TeacherId", "TimeLimit", "Title", "TotalScore" },
                values: new object[] { 1, 3, new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), false, 2, null, 1, 2, 30, "Algebra Quiz", 100 });

            migrationBuilder.InsertData(
                table: "lessonPlans",
                columns: new[] { "LessonPlanId", "AiRequestId", "Content", "CreatedAt", "Duration", "ExportPath", "LevelId", "PublishedAt", "Status", "TeacherId", "Title", "UpdatedAt", "Version" },
                values: new object[] { 1, 1, "Introduction to Algebra", new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), 60, null, 2, null, 1, 2, "Algebra Basics", null, 1 });

            migrationBuilder.InsertData(
                table: "questions",
                columns: new[] { "QuestionId", "AiRequestId", "CorrectAnswer", "CreatedAt", "DifficultyId", "Explanation", "IsAIGenerated", "QuestionBankId", "QuestionText", "QuestionType", "QuizId", "Status", "Tags", "Topic", "UpdatedAt" },
                values: new object[] { 1, 1, "2", new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), 1, "Divide both sides by 2.", false, 1, "What is x in 2x=4?", 0, 1, 1, "algebra,equation", 3, null });

            migrationBuilder.InsertData(
                table: "submissions",
                columns: new[] { "SubmissionId", "AttemptNumber", "DurationTaken", "Feedback", "QuizId", "Score", "Status", "StudentId", "SubmittedAt" },
                values: new object[] { 1, 1, 1200, "Great job!", 1, 100m, 0, 1, new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "answers",
                columns: new[] { "AnswerId", "AnswerText", "IsCorrect", "QuestionId" },
                values: new object[,]
                {
                    { 1, "2", true, 1 },
                    { 2, "4", false, 1 }
                });

            migrationBuilder.InsertData(
                table: "lessons",
                columns: new[] { "LessonId", "Content", "IsShared", "LessonPlanId", "PublishedDate", "Title" },
                values: new object[] { 1, "Algebra is ...", true, 1, new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), "What is Algebra?" });

            migrationBuilder.InsertData(
                table: "submissionDetails",
                columns: new[] { "SubmissionDetailId", "Explanation", "IsCorrect", "QuestionId", "ScoreEarned", "StudentAnswer", "SubmissionId" },
                values: new object[] { 1, "Correct answer.", true, 1, 100m, "2", 1 });

            migrationBuilder.InsertData(
                table: "lessonDetails",
                columns: new[] { "LessonDetailId", "Content", "ContentLaTeX", "ContentType", "CreatedAt", "LessonId", "Order", "UpdatedAt" },
                values: new object[] { 1, "Algebra is a branch of mathematics ...", null, 0, new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), 1, 1, null });

            migrationBuilder.InsertData(
                table: "progresses",
                columns: new[] { "ProgressId", "AttemptDate", "CompletionStatus", "IsActive", "LessonId", "StudentId" },
                values: new object[] { 1, new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), 2, true, 1, 1 });

            migrationBuilder.InsertData(
                table: "attachments",
                columns: new[] { "AttachmentId", "FileName", "FilePath", "FileSize", "FileType", "LessonDetailId", "UploadTimestamp" },
                values: new object[] { 1, "intro.pdf", "/files/intro.pdf", 102400L, "pdf", 1, new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_AiRequest_LevelId",
                table: "AiRequest",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_AiRequest_UserId",
                table: "AiRequest",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_answers_QuestionId",
                table: "answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_attachments_LessonDetailId",
                table: "attachments",
                column: "LessonDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_auditLogs_UserId",
                table: "auditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_lessonDetails_LessonId",
                table: "lessonDetails",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_lessonPlans_AiRequestId",
                table: "lessonPlans",
                column: "AiRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_lessonPlans_LevelId",
                table: "lessonPlans",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_lessonPlans_TeacherId",
                table: "lessonPlans",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_LessonPlanId",
                table: "lessons",
                column: "LessonPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_progresses_LessonId",
                table: "progresses",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_progresses_StudentId",
                table: "progresses",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_questionBanks_LevelId",
                table: "questionBanks",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_questionBanks_TeacherId",
                table: "questionBanks",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_questions_AiRequestId",
                table: "questions",
                column: "AiRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_questions_DifficultyId",
                table: "questions",
                column: "DifficultyId");

            migrationBuilder.CreateIndex(
                name: "IX_questions_QuestionBankId",
                table: "questions",
                column: "QuestionBankId");

            migrationBuilder.CreateIndex(
                name: "IX_questions_QuizId",
                table: "questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_LevelId",
                table: "quizzes",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_TeacherId",
                table: "quizzes",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_submissionDetails_QuestionId",
                table: "submissionDetails",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_submissionDetails_SubmissionId",
                table: "submissionDetails",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_submissions_QuizId",
                table: "submissions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_submissions_StudentId",
                table: "submissions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_users_LevelId",
                table: "users",
                column: "LevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answers");

            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropTable(
                name: "auditLogs");

            migrationBuilder.DropTable(
                name: "progresses");

            migrationBuilder.DropTable(
                name: "submissionDetails");

            migrationBuilder.DropTable(
                name: "lessonDetails");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "submissions");

            migrationBuilder.DropTable(
                name: "lessons");

            migrationBuilder.DropTable(
                name: "difficulties");

            migrationBuilder.DropTable(
                name: "questionBanks");

            migrationBuilder.DropTable(
                name: "quizzes");

            migrationBuilder.DropTable(
                name: "lessonPlans");

            migrationBuilder.DropTable(
                name: "AiRequest");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "levels");
        }
    }
}
