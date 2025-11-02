using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace repositories.Migrations
{
    /// <inheritdoc />
    public partial class FixEnumValuesExplicit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_lessonDetails_LessonId",
                table: "lessonDetails");

            migrationBuilder.CreateIndex(
                name: "IX_LessonDetail_LessonId_Order",
                table: "lessonDetails",
                columns: new[] { "LessonId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lessonDetails_ContentType",
                table: "lessonDetails",
                column: "ContentType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LessonDetail_LessonId_Order",
                table: "lessonDetails");

            migrationBuilder.DropIndex(
                name: "IX_lessonDetails_ContentType",
                table: "lessonDetails");

            migrationBuilder.CreateIndex(
                name: "IX_lessonDetails_LessonId",
                table: "lessonDetails",
                column: "LessonId");
        }
    }
}
