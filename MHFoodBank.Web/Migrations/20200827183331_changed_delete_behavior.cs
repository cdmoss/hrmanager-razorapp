using Microsoft.EntityFrameworkCore.Migrations;

namespace MHFoodBank.Web.Migrations
{
    public partial class changed_delete_behavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PositionVolunteers_Positions_PositionId",
                table: "PositionVolunteers");

            migrationBuilder.AddForeignKey(
                name: "FK_PositionVolunteers_Positions_PositionId",
                table: "PositionVolunteers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PositionVolunteers_Positions_PositionId",
                table: "PositionVolunteers");

            migrationBuilder.AddForeignKey(
                name: "FK_PositionVolunteers_Positions_PositionId",
                table: "PositionVolunteers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
