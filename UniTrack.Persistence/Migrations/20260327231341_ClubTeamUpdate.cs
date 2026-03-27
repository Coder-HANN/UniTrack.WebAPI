using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniTrack.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ClubTeamUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubTeams_UserDetails_UserDetailId",
                table: "ClubTeams");

            migrationBuilder.DropIndex(
                name: "IX_ClubTeams_UserDetailId",
                table: "ClubTeams");

            migrationBuilder.DropColumn(
                name: "UserDetailId",
                table: "ClubTeams");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ClubTeams",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ClubTeams",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ClubTeams_UserId",
                table: "ClubTeams",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubTeams_Users_UserId",
                table: "ClubTeams",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubTeams_Users_UserId",
                table: "ClubTeams");

            migrationBuilder.DropIndex(
                name: "IX_ClubTeams_UserId",
                table: "ClubTeams");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ClubTeams");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ClubTeams",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "UserDetailId",
                table: "ClubTeams",
                type: "uniqueidentifier",
                maxLength: 50,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ClubTeams_UserDetailId",
                table: "ClubTeams",
                column: "UserDetailId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubTeams_UserDetails_UserDetailId",
                table: "ClubTeams",
                column: "UserDetailId",
                principalTable: "UserDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
