namespace LetsSport.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class RemoveUserDbSetHiddingInheritedOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Sporters_AdminId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_EventsSporters_Sporters_SporterId",
                table: "EventsSporters");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Sporters_SenderId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sporters",
                table: "Sporters");

            migrationBuilder.RenameTable(
                name: "Sporters",
                newName: "User");

            migrationBuilder.RenameIndex(
                name: "IX_Sporters_Email",
                table: "User",
                newName: "IX_User_Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_User_AdminId",
                table: "Events",
                column: "AdminId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventsSporters_User_SporterId",
                table: "EventsSporters",
                column: "SporterId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_User_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_User_AdminId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_EventsSporters_User_SporterId",
                table: "EventsSporters");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_User_SenderId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Sporters");

            migrationBuilder.RenameIndex(
                name: "IX_User_Email",
                table: "Sporters",
                newName: "IX_Sporters_Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sporters",
                table: "Sporters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Sporters_AdminId",
                table: "Events",
                column: "AdminId",
                principalTable: "Sporters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventsSporters_Sporters_SporterId",
                table: "EventsSporters",
                column: "SporterId",
                principalTable: "Sporters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Sporters_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "Sporters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
