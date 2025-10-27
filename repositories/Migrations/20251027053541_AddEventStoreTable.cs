using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddEventStoreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiRequest_levels_LevelId",
                table: "AiRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_AiRequest_users_UserId",
                table: "AiRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_lessonPlans_AiRequest_AiRequestId",
                table: "lessonPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_questions_AiRequest_AiRequestId",
                table: "questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AiRequest",
                table: "AiRequest");

            migrationBuilder.RenameTable(
                name: "AiRequest",
                newName: "AiRequests");

            migrationBuilder.RenameIndex(
                name: "IX_AiRequest_UserId",
                table: "AiRequests",
                newName: "IX_AiRequests_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AiRequest_LevelId",
                table: "AiRequests",
                newName: "IX_AiRequests_LevelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AiRequests",
                table: "AiRequests",
                column: "AIRequestId");

            migrationBuilder.CreateTable(
                name: "EventStore",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    AggregateType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStore", x => x.EventId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AiRequests_levels_LevelId",
                table: "AiRequests",
                column: "LevelId",
                principalTable: "levels",
                principalColumn: "LevelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AiRequests_users_UserId",
                table: "AiRequests",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_lessonPlans_AiRequests_AiRequestId",
                table: "lessonPlans",
                column: "AiRequestId",
                principalTable: "AiRequests",
                principalColumn: "AIRequestId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_questions_AiRequests_AiRequestId",
                table: "questions",
                column: "AiRequestId",
                principalTable: "AiRequests",
                principalColumn: "AIRequestId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AiRequests_levels_LevelId",
                table: "AiRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AiRequests_users_UserId",
                table: "AiRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_lessonPlans_AiRequests_AiRequestId",
                table: "lessonPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_questions_AiRequests_AiRequestId",
                table: "questions");

            migrationBuilder.DropTable(
                name: "EventStore");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AiRequests",
                table: "AiRequests");

            migrationBuilder.RenameTable(
                name: "AiRequests",
                newName: "AiRequest");

            migrationBuilder.RenameIndex(
                name: "IX_AiRequests_UserId",
                table: "AiRequest",
                newName: "IX_AiRequest_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AiRequests_LevelId",
                table: "AiRequest",
                newName: "IX_AiRequest_LevelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AiRequest",
                table: "AiRequest",
                column: "AIRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_AiRequest_levels_LevelId",
                table: "AiRequest",
                column: "LevelId",
                principalTable: "levels",
                principalColumn: "LevelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AiRequest_users_UserId",
                table: "AiRequest",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_lessonPlans_AiRequest_AiRequestId",
                table: "lessonPlans",
                column: "AiRequestId",
                principalTable: "AiRequest",
                principalColumn: "AIRequestId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_questions_AiRequest_AiRequestId",
                table: "questions",
                column: "AiRequestId",
                principalTable: "AiRequest",
                principalColumn: "AIRequestId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
