using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MHFoodBank.Web.Migrations
{
    public partial class change_to_scheduler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Shifts_ParentRecurringShiftId",
                table: "Shifts");

            migrationBuilder.DropTable(
                name: "ShiftLinks");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_ParentRecurringShiftId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "Weekdays",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "Hidden",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ParentRecurringShiftId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Shifts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Shifts",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Shifts",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time(6)");

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceException",
                table: "Shifts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceID",
                table: "Shifts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecurrenceException",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "RecurrenceID",
                table: "Shifts");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "Shifts",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "Shifts",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Shifts",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Weekdays",
                table: "Shifts",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Shifts",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Hidden",
                table: "Shifts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentRecurringShiftId",
                table: "Shifts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Shifts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ShiftLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NewShiftId = table.Column<int>(type: "int", nullable: true),
                    OriginalShiftId = table.Column<int>(type: "int", nullable: true),
                    ParentSetId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftLinks_Shifts_NewShiftId",
                        column: x => x.NewShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftLinks_Shifts_OriginalShiftId",
                        column: x => x.OriginalShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftLinks_Shifts_ParentSetId",
                        column: x => x.ParentSetId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_ParentRecurringShiftId",
                table: "Shifts",
                column: "ParentRecurringShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLinks_NewShiftId",
                table: "ShiftLinks",
                column: "NewShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLinks_OriginalShiftId",
                table: "ShiftLinks",
                column: "OriginalShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftLinks_ParentSetId",
                table: "ShiftLinks",
                column: "ParentSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Shifts_ParentRecurringShiftId",
                table: "Shifts",
                column: "ParentRecurringShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
