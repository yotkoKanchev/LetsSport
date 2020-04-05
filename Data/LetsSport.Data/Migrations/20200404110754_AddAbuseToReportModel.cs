﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class AddAbuseToReportModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Abuse",
                table: "Reports",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abuse",
                table: "Reports");
        }
    }
}