using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniTrack.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OpportuniesEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "Opportunitys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OpportunityUniversities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpportunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniversityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityUniversities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunityUniversities_Opportunitys_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunitys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityUniversities_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityUniversities_OpportunityId",
                table: "OpportunityUniversities",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityUniversities_UniversityId",
                table: "OpportunityUniversities",
                column: "UniversityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpportunityUniversities");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Opportunitys");
        }
    }
}
