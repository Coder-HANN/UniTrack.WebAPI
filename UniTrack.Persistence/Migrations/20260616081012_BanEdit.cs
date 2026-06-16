using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniTrack.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BanEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bans_ClubId",
                table: "Bans");

            migrationBuilder.DropIndex(
                name: "IX_Bans_EventId",
                table: "Bans");

            migrationBuilder.DropIndex(
                name: "IX_Bans_UserId",
                table: "Bans");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_ClubId",
                table: "Bans",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_EventId",
                table: "Bans",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_UserId",
                table: "Bans",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bans_ClubId",
                table: "Bans");

            migrationBuilder.DropIndex(
                name: "IX_Bans_EventId",
                table: "Bans");

            migrationBuilder.DropIndex(
                name: "IX_Bans_UserId",
                table: "Bans");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_ClubId",
                table: "Bans",
                column: "ClubId",
                unique: true,
                filter: "[ClubId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_EventId",
                table: "Bans",
                column: "EventId",
                unique: true,
                filter: "[EventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_UserId",
                table: "Bans",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }
    }
}
