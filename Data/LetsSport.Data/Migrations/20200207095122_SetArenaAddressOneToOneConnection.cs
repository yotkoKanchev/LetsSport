namespace LetsSport.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class SetArenaAddressOneToOneConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sporters_IsDeleted",
                table: "Sporters");

            migrationBuilder.DropIndex(
                name: "IX_Arenas_AddressId",
                table: "Arenas");

            migrationBuilder.DropIndex(
                name: "IX_ArenaAdmins_IsDeleted",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "ArenaAdmins");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Sporters",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "ArenaAdmins",
                newName: "UserName");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Sporters",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Sporters",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Sporters",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Sporters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Sporters",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Sporters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Sporters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Sporters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Sporters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Sporters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Sporters",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Sporters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Sporters",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Sporters",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "ArenaAdmins",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "ArenaAdmins",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "ArenaAdmins",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ArenaAdmins",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "ArenaAdmins",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "ArenaAdmins",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "ArenaAdmins",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "ArenaAdmins",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "ArenaAdmins",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "ArenaAdmins",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "ArenaAdmins",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "ArenaAdmins",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "ArenaAdmins",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "ArenaAdmins",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Sporters_Email",
                table: "Sporters",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Arenas_AddressId",
                table: "Arenas",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArenaAdmins_Email",
                table: "ArenaAdmins",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sporters_Email",
                table: "Sporters");

            migrationBuilder.DropIndex(
                name: "IX_Arenas_AddressId",
                table: "Arenas");

            migrationBuilder.DropIndex(
                name: "IX_ArenaAdmins_Email",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Sporters");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "ArenaAdmins");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "ArenaAdmins");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Sporters",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "ArenaAdmins",
                newName: "Username");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Sporters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Sporters",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Sporters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Sporters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Sporters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Sporters",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Sporters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Sporters",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Sporters",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "ArenaAdmins",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "ArenaAdmins",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "ArenaAdmins",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ArenaAdmins",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ArenaAdmins",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ArenaAdmins",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "ArenaAdmins",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ArenaAdmins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "ArenaAdmins",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ArenaAdmins",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sporters_IsDeleted",
                table: "Sporters",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Arenas_AddressId",
                table: "Arenas",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ArenaAdmins_IsDeleted",
                table: "ArenaAdmins",
                column: "IsDeleted");
        }
    }
}
