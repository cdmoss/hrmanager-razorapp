using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkplaceAdministrator.Api.Migrations
{
    public partial class fixroles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("206f3af5-2fa0-40ff-b95a-303a7c24ddc6"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("222f0447-e59b-4cd5-8027-e97110df82bf"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3a9b9375-4db9-4597-adad-3eda2f1cd29b"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("6e25126f-c1ad-4df5-beb5-f0031a76fa0b"), "5e167ce4-eeb9-43cd-bf4f-3031f7c753d9", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("a876610f-fa74-4038-9ac1-7cd012d056a1"), "abe17957-9e30-4432-b806-8a3902a378c8", "SuperAdmin", "SUPERADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("3a463fae-41c4-49ea-96b0-7663772c615f"), "bbb0fc3b-d8b4-4463-9d83-ad9f2a3f4eb4", "Employee", "EMPLOYEE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3a463fae-41c4-49ea-96b0-7663772c615f"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("6e25126f-c1ad-4df5-beb5-f0031a76fa0b"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a876610f-fa74-4038-9ac1-7cd012d056a1"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("3a9b9375-4db9-4597-adad-3eda2f1cd29b"), "e03caaf6-3f08-48de-b840-c2056188f5e9", "Member", "Member" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("206f3af5-2fa0-40ff-b95a-303a7c24ddc6"), "5e6452e9-b40b-4d40-9302-d3c589cb4637", "Staff", "STAFF" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("222f0447-e59b-4cd5-8027-e97110df82bf"), "df38e007-b485-4c86-b8e4-eb33427808ed", "Admin", "ADMIN" });
        }
    }
}
