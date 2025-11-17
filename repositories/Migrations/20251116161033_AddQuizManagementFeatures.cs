using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizManagementFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 1,
                column: "LevelName",
                value: "Grade 1");

            migrationBuilder.UpdateData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "EducationLevel", "LevelName" },
                values: new object[] { 0, "Grade 2" });

            migrationBuilder.UpdateData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "EducationLevel", "LevelName" },
                values: new object[] { 0, "Grade 3" });

            migrationBuilder.InsertData(
                table: "levels",
                columns: new[] { "LevelId", "EducationLevel", "LevelName", "Order" },
                values: new object[,]
                {
                    { 4, 0, "Grade 4", 4 },
                    { 5, 0, "Grade 5", 5 },
                    { 6, 1, "Grade 6", 6 },
                    { 7, 1, "Grade 7", 7 },
                    { 8, 1, "Grade 8", 8 },
                    { 9, 1, "Grade 9", 9 },
                    { 10, 2, "Grade 10", 10 },
                    { 11, 2, "Grade 11", 11 },
                    { 12, 2, "Grade 12", 12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 12);

            migrationBuilder.UpdateData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 1,
                column: "LevelName",
                value: "Primary School");

            migrationBuilder.UpdateData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 2,
                columns: new[] { "EducationLevel", "LevelName" },
                values: new object[] { 1, "Secondary School" });

            migrationBuilder.UpdateData(
                table: "levels",
                keyColumn: "LevelId",
                keyValue: 3,
                columns: new[] { "EducationLevel", "LevelName" },
                values: new object[] { 2, "High School" });
        }
    }
}
