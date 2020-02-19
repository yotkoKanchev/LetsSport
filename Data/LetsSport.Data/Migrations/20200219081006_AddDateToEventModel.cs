using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class AddDateToEventModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndingHour",
                table: "Events");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Events",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Events");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndingHour",
                table: "Events",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
