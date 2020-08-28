using Microsoft.EntityFrameworkCore.Migrations;

namespace MHFoodBank.Web.Migrations
{
    public partial class changed_shiftalert_deletebehaviour : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Shifts_OriginalShiftId",
                table: "Alerts");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Shifts_OriginalShiftId",
                table: "Alerts",
                column: "OriginalShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Shifts_OriginalShiftId",
                table: "Alerts");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Shifts_OriginalShiftId",
                table: "Alerts",
                column: "OriginalShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
