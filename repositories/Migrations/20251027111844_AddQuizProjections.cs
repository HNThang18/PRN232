using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizProjections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuizListProjection",
                columns: table => new
                {
                    AggregateId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GradeLevel = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    QuestionCount = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsFailed = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AiRequestId = table.Column<int>(type: "int", nullable: true),
                    InitiatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessingDuration = table.Column<double>(type: "float", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizListProjection", x => x.AggregateId);
                });

            migrationBuilder.CreateTable(
                name: "QuizStatisticsProjection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalInitiated = table.Column<int>(type: "int", nullable: false),
                    TotalCompleted = table.Column<int>(type: "int", nullable: false),
                    TotalFailed = table.Column<int>(type: "int", nullable: false),
                    SuccessRate = table.Column<double>(type: "float", nullable: false),
                    AverageDuration = table.Column<double>(type: "float", nullable: false),
                    MinDuration = table.Column<double>(type: "float", nullable: false),
                    MaxDuration = table.Column<double>(type: "float", nullable: false),
                    TotalQuestionsGenerated = table.Column<int>(type: "int", nullable: false),
                    AverageQuestionsPerQuiz = table.Column<double>(type: "float", nullable: false),
                    GradeLevelDistribution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TopTopics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizStatisticsProjection", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizListProjection");

            migrationBuilder.DropTable(
                name: "QuizStatisticsProjection");
        }
    }
}
