using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class UpdateEventToChatRoomOneToOneConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_Events_EventId",
                table: "ChatRooms");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_EventId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "ChatRooms");

            migrationBuilder.AddColumn<string>(
                name: "ChatRoomId",
                table: "Events",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ChatRoomId",
                table: "Events",
                column: "ChatRoomId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_ChatRooms_ChatRoomId",
                table: "Events",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_ChatRooms_ChatRoomId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ChatRoomId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ChatRoomId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "ChatRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_EventId",
                table: "ChatRooms",
                column: "EventId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_Events_EventId",
                table: "ChatRooms",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
