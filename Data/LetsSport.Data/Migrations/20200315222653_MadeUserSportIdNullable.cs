using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class MadeUserSportIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SportId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "SportId",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SportId",
                table: "AspNetUsers",
                column: "SportId",
                unique: true,
                filter: "[SportId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SportId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "SportId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SportId",
                table: "AspNetUsers",
                column: "SportId",
                unique: true);
        }
    }
}
