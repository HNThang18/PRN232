using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace repositories.Migrations
{
    /// <inheritdoc />
    public partial class FixLessonSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "lessons",
                keyColumn: "LessonId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Order" },
                values: new object[] { new DateTime(2024, 6, 14, 8, 51, 0, 0, DateTimeKind.Utc), 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "lessons",
                keyColumn: "LessonId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Order" },
                values: new object[] { new DateTime(2025, 10, 26, 23, 45, 50, 330, DateTimeKind.Utc).AddTicks(2709), 0 });
        }
    }
}
