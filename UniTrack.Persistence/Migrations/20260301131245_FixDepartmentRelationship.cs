using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniTrack.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixDepartmentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDetails_DepartmentId",
                table: "UserDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_DepartmentId",
                table: "UserDetails",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDetails_DepartmentId",
                table: "UserDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_DepartmentId",
                table: "UserDetails",
                column: "DepartmentId",
                unique: true);
        }
    }
}
