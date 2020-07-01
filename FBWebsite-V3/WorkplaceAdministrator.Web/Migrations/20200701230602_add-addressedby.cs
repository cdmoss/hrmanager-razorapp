using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkplaceAdministrator.Web.Migrations
{
    public partial class addaddressedby : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressedBy",
                table: "Alerts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClockedTime",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: true),
                    PositionId = table.Column<int>(nullable: true),
                    VolunteerProfileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClockedTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClockedTime_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClockedTime_VolunteerProfiles_VolunteerProfileId",
                        column: x => x.VolunteerProfileId,
                        principalTable: "VolunteerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClockedTime_PositionId",
                table: "ClockedTime",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClockedTime_VolunteerProfileId",
                table: "ClockedTime",
                column: "VolunteerProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClockedTime");

            migrationBuilder.DropColumn(
                name: "AddressedBy",
                table: "Alerts");
        }
    }
}
