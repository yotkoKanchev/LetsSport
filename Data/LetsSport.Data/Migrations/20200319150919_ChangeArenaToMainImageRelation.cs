using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class ChangeArenaToMainImageRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Arenas_ArenaId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ArenaId",
                table: "Images");

            migrationBuilder.AddColumn<int>(
                name: "ArenaId1",
                table: "Images",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_ArenaId1",
                table: "Images",
                column: "ArenaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Arenas_ArenaId1",
                table: "Images",
                column: "ArenaId1",
                principalTable: "Arenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Arenas_ArenaId1",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ArenaId1",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ArenaId1",
                table: "Images");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ArenaId",
                table: "Images",
                column: "ArenaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Arenas_ArenaId",
                table: "Images",
                column: "ArenaId",
                principalTable: "Arenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
