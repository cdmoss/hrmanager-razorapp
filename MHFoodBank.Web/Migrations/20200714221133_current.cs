using Microsoft.EntityFrameworkCore.Migrations;

namespace MHFoodBank.Web.Migrations
{
    public partial class current : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlertType",
                table: "Alerts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertType",
                table: "Alerts");
        }
    }
}
