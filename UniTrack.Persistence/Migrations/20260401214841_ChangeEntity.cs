using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniTrack.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Universities_UniversityId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_UniversityId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "UniversityId",
                table: "Departments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UniversityId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_UniversityId",
                table: "Departments",
                column: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Universities_UniversityId",
                table: "Departments",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
