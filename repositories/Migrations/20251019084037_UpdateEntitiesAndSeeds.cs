using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntitiesAndSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_progresses_lessons_LessonId",
                table: "progresses");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Example",
                table: "lessons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGeneratedByAI",
                table: "lessons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Objective",
                table: "lessons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ResourceUrl",
                table: "lessons",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "lessons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiPrompt",
                table: "lessonPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AiResponse",
                table: "lessonPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "lessonPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "lessonPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LearningObjectives",
                table: "lessonPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MathFormulas",
                table: "lessonPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "lessonPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "lessonPlans",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "attachments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "attachments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "UploadedBy",
                table: "attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "attachments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "attachments",
                keyColumn: "AttachmentId",
                keyValue: 1,
                columns: new[] { "UploadedBy", "UserId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "lessonPlans",
                keyColumn: "LessonPlanId",
                keyValue: 1,
                columns: new[] { "AiPrompt", "AiResponse", "Grade", "IsPublic", "LearningObjectives", "MathFormulas", "Tags", "Topic" },
                values: new object[] { null, null, 0, false, null, null, null, "Algebra" });

            migrationBuilder.UpdateData(
                table: "lessons",
                keyColumn: "LessonId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Example", "IsGeneratedByAI", "Objective", "Order", "ResourceUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 19, 8, 40, 36, 640, DateTimeKind.Utc).AddTicks(6333), null, false, null, 0, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_attachments_UserId",
                table: "attachments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_attachments_users_UserId",
                table: "attachments",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_progresses_lessons_LessonId",
                table: "progresses",
                column: "LessonId",
                principalTable: "lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attachments_users_UserId",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_progresses_lessons_LessonId",
                table: "progresses");

            migrationBuilder.DropIndex(
                name: "IX_attachments_UserId",
                table: "attachments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "Example",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "IsGeneratedByAI",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "Objective",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "ResourceUrl",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "AiPrompt",
                table: "lessonPlans");

            migrationBuilder.DropColumn(
                name: "AiResponse",
                table: "lessonPlans");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "lessonPlans");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "lessonPlans");

            migrationBuilder.DropColumn(
                name: "LearningObjectives",
                table: "lessonPlans");

            migrationBuilder.DropColumn(
                name: "MathFormulas",
                table: "lessonPlans");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "lessonPlans");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "lessonPlans");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "attachments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "attachments");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "attachments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "attachments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddForeignKey(
                name: "FK_progresses_lessons_LessonId",
                table: "progresses",
                column: "LessonId",
                principalTable: "lessons",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
