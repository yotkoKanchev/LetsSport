using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class RemoveRatingPropFromArena : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arenas_Addresses_AddressId",
                table: "Arenas");

            migrationBuilder.DropIndex(
                name: "IX_Arenas_AddressId",
                table: "Arenas");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Arenas");

            migrationBuilder.AddColumn<int>(
                name: "ArenaId",
                table: "Addresses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ArenaId",
                table: "Addresses",
                column: "ArenaId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Arenas_ArenaId",
                table: "Addresses",
                column: "ArenaId",
                principalTable: "Arenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Arenas_ArenaId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_ArenaId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "ArenaId",
                table: "Addresses");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Countries",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Arenas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Arenas_AddressId",
                table: "Arenas",
                column: "AddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Arenas_Addresses_AddressId",
                table: "Arenas",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
