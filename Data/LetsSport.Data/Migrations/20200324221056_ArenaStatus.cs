using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class ArenaStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Arenas",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Arenas");
        }
    }
}
