using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace repositories.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditLogFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EntityId",
                table: "auditLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "auditLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "auditLogs",
                keyColumn: "LogId",
                keyValue: 1,
                column: "IpAddress",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "auditLogs");

            migrationBuilder.AlterColumn<int>(
                name: "EntityId",
                table: "auditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
