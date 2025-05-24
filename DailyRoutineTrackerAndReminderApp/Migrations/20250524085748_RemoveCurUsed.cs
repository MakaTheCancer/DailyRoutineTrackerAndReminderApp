using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyRoutineTrackerAndReminderApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCurUsed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrentUsers",
                columns: table => new
                {
                    CurrentUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentUsers", x => x.CurrentUserId);
                });
        }
    }
}
