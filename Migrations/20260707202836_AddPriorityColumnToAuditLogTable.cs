using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureAuthDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityColumnToAuditLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "AuditLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "AuditLogs");
        }
    }
}
