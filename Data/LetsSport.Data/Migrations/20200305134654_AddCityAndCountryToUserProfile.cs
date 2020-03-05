using Microsoft.EntityFrameworkCore.Migrations;

namespace LetsSport.Data.Migrations
{
    public partial class AddCityAndCountryToUserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "UserProfile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "UserProfile",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_CityId",
                table: "UserProfile",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_CountryId",
                table: "UserProfile",
                column: "CountryId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Cities_CityId",
                table: "UserProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfile_Countries_CountryId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_CityId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_CountryId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "UserProfile");
        }
    }
}
