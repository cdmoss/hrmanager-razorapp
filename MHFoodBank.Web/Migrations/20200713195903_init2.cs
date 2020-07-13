using Microsoft.EntityFrameworkCore.Migrations;

namespace MHFoodBank.Web.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Shifts_NewShiftId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Shifts_OldShiftId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_NewShiftId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_OldShiftId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "NewShiftId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "OldShiftId",
                table: "Alerts");

            migrationBuilder.AddColumn<int>(
                name: "OriginalShiftId",
                table: "Alerts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestedShiftId",
                table: "Alerts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_OriginalShiftId",
                table: "Alerts",
                column: "OriginalShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_RequestedShiftId",
                table: "Alerts",
                column: "RequestedShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Shifts_OriginalShiftId",
                table: "Alerts",
                column: "OriginalShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Shifts_RequestedShiftId",
                table: "Alerts",
                column: "RequestedShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Shifts_OriginalShiftId",
                table: "Alerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Alerts_Shifts_RequestedShiftId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_OriginalShiftId",
                table: "Alerts");

            migrationBuilder.DropIndex(
                name: "IX_Alerts_RequestedShiftId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "OriginalShiftId",
                table: "Alerts");

            migrationBuilder.DropColumn(
                name: "RequestedShiftId",
                table: "Alerts");

            migrationBuilder.AddColumn<int>(
                name: "NewShiftId",
                table: "Alerts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldShiftId",
                table: "Alerts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_NewShiftId",
                table: "Alerts",
                column: "NewShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_OldShiftId",
                table: "Alerts",
                column: "OldShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Shifts_NewShiftId",
                table: "Alerts",
                column: "NewShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Alerts_Shifts_OldShiftId",
                table: "Alerts",
                column: "OldShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
