namespace LetsSport.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class RenameEventsSportersToEventsUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventsSporters_Events_EventId",
                table: "EventsSporters");

            migrationBuilder.DropForeignKey(
                name: "FK_EventsSporters_User_SporterId",
                table: "EventsSporters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventsSporters",
                table: "EventsSporters");

            migrationBuilder.RenameTable(
                name: "EventsSporters",
                newName: "EventsUsers");

            migrationBuilder.RenameIndex(
                name: "IX_EventsSporters_SporterId",
                table: "EventsUsers",
                newName: "IX_EventsUsers_SporterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventsUsers",
                table: "EventsUsers",
                columns: new[] { "EventId", "SporterId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EventsUsers_Events_EventId",
                table: "EventsUsers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventsUsers_User_SporterId",
                table: "EventsUsers",
                column: "SporterId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventsUsers_Events_EventId",
                table: "EventsUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventsUsers_User_SporterId",
                table: "EventsUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventsUsers",
                table: "EventsUsers");

            migrationBuilder.RenameTable(
                name: "EventsUsers",
                newName: "EventsSporters");

            migrationBuilder.RenameIndex(
                name: "IX_EventsUsers_SporterId",
                table: "EventsSporters",
                newName: "IX_EventsSporters_SporterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventsSporters",
                table: "EventsSporters",
                columns: new[] { "EventId", "SporterId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EventsSporters_Events_EventId",
                table: "EventsSporters",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventsSporters_User_SporterId",
                table: "EventsSporters",
                column: "SporterId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
