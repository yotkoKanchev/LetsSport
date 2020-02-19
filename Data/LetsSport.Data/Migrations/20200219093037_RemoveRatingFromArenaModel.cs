using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class RemoveRatingFromArenaModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Arenas");

            migrationBuilder.AddColumn<int>(
                name: "Sport",
                table: "Arenas",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sport",
                table: "Arenas");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Arenas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
