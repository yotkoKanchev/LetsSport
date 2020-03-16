﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class ChangeArenaSportRelationToOneToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Arenas_SportId",
                table: "Arenas");

            migrationBuilder.CreateIndex(
                name: "IX_Arenas_SportId",
                table: "Arenas",
                column: "SportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Arenas_SportId",
                table: "Arenas");

            migrationBuilder.CreateIndex(
                name: "IX_Arenas_SportId",
                table: "Arenas",
                column: "SportId",
                unique: true);
        }
    }
}
