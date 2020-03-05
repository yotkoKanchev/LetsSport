using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class RemoveArenaAdminModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_AspNetUsers_ApplicationUserId",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Cities_CityId",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Countries_CountryId",
                table: "UserProfile");

            migrationBuilder.DropTable(
                name: "ArenaAdmin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile");

            migrationBuilder.RenameTable(
                name: "UserProfile",
                newName: "UserProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfile_IsDeleted",
                table: "UserProfiles",
                newName: "IX_UserProfiles_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfile_CountryId",
                table: "UserProfiles",
                newName: "IX_UserProfiles_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfile_CityId",
                table: "UserProfiles",
                newName: "IX_UserProfiles_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfile_ApplicationUserId",
                table: "UserProfiles",
                newName: "IX_UserProfiles_ApplicationUserId");

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "UserProfiles",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfiles",
                table: "UserProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_AspNetUsers_ApplicationUserId",
                table: "UserProfiles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Cities_CityId",
                table: "UserProfiles",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Countries_CountryId",
                table: "UserProfiles",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_AspNetUsers_ApplicationUserId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Cities_CityId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Countries_CountryId",
                table: "UserProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfiles",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "UserProfiles");

            migrationBuilder.RenameTable(
                name: "UserProfiles",
                newName: "UserProfile");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfiles_IsDeleted",
                table: "UserProfile",
                newName: "IX_UserProfile_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfiles_CountryId",
                table: "UserProfile",
                newName: "IX_UserProfile_CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfiles_CityId",
                table: "UserProfile",
                newName: "IX_UserProfile_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfiles_ApplicationUserId",
                table: "UserProfile",
                newName: "IX_UserProfile_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ArenaAdmin",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaAdmin_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArenaAdmin_ApplicationUserId",
                table: "ArenaAdmin",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArenaAdmin_IsDeleted",
                table: "ArenaAdmin",
                column: "IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_AspNetUsers_ApplicationUserId",
                table: "UserProfile",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Cities_CityId",
                table: "UserProfile",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfile_Countries_CountryId",
                table: "UserProfile",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
