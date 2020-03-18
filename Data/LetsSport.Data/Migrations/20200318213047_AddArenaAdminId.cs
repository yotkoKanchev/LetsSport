using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class AddArenaAdminId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Arenas_AdministratingArenaId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AdministratingArenaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AdministratingArenaId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ArenaAdminId",
                table: "Arenas",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Arenas_ArenaAdminId",
                table: "Arenas",
                column: "ArenaAdminId",
                unique: true,
                filter: "[ArenaAdminId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Arenas_AspNetUsers_ArenaAdminId",
                table: "Arenas",
                column: "ArenaAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arenas_AspNetUsers_ArenaAdminId",
                table: "Arenas");

            migrationBuilder.DropIndex(
                name: "IX_Arenas_ArenaAdminId",
                table: "Arenas");

            migrationBuilder.DropColumn(
                name: "ArenaAdminId",
                table: "Arenas");

            migrationBuilder.AddColumn<int>(
                name: "AdministratingArenaId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AdministratingArenaId",
                table: "AspNetUsers",
                column: "AdministratingArenaId",
                unique: true,
                filter: "[AdministratingArenaId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Arenas_AdministratingArenaId",
                table: "AspNetUsers",
                column: "AdministratingArenaId",
                principalTable: "Arenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
