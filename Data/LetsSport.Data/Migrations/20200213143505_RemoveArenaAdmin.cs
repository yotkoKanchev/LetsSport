using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class RemoveArenaAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventsUsers_User_SporterId",
                table: "EventsUsers");

            migrationBuilder.DropTable(
                name: "ArenaAdmins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventsUsers",
                table: "EventsUsers");

            migrationBuilder.DropIndex(
                name: "IX_EventsUsers_SporterId",
                table: "EventsUsers");

            migrationBuilder.DropColumn(
                name: "SporterId",
                table: "EventsUsers");

            migrationBuilder.AddColumn<int>(
                name: "ArenaId",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "User",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "EventsUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventsUsers",
                table: "EventsUsers",
                columns: new[] { "EventId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_User_ArenaId",
                table: "User",
                column: "ArenaId",
                unique: true,
                filter: "[ArenaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventsUsers_UserId",
                table: "EventsUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventsUsers_User_UserId",
                table: "EventsUsers",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Arenas_ArenaId",
                table: "User",
                column: "ArenaId",
                principalTable: "Arenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventsUsers_User_UserId",
                table: "EventsUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Arenas_ArenaId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ArenaId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventsUsers",
                table: "EventsUsers");

            migrationBuilder.DropIndex(
                name: "IX_EventsUsers_UserId",
                table: "EventsUsers");

            migrationBuilder.DropColumn(
                name: "ArenaId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EventsUsers");

            migrationBuilder.AddColumn<string>(
                name: "SporterId",
                table: "EventsUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventsUsers",
                table: "EventsUsers",
                columns: new[] { "EventId", "SporterId" });

            migrationBuilder.CreateTable(
                name: "ArenaAdmins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ArenaId = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArenaAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArenaAdmins_Arenas_ArenaId",
                        column: x => x.ArenaId,
                        principalTable: "Arenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventsUsers_SporterId",
                table: "EventsUsers",
                column: "SporterId");

            migrationBuilder.CreateIndex(
                name: "IX_ArenaAdmins_ArenaId",
                table: "ArenaAdmins",
                column: "ArenaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArenaAdmins_Email",
                table: "ArenaAdmins",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_EventsUsers_User_SporterId",
                table: "EventsUsers",
                column: "SporterId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
