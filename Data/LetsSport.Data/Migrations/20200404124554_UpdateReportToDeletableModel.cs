using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class UpdateReportToDeletableModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReportedUserId",
                table: "Reports",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Reports",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reports",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_IsDeleted",
                table: "Reports",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportedUserId",
                table: "Reports",
                column: "ReportedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ReportedUserId",
                table: "Reports",
                column: "ReportedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ReportedUserId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_IsDeleted",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportedUserId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reports");

            migrationBuilder.AlterColumn<string>(
                name: "ReportedUserId",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
