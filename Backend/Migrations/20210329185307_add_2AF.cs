using Microsoft.EntityFrameworkCore.Migrations;

namespace PortableManager.Web.Server.Migrations
{
    public partial class add_2AF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0007f4df-8861-4993-bcd6-6f9ddb4be848");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4ebe7abc-54c5-4bf9-ac0b-7f7def2fe2a4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "628bb844-ea83-4661-bce1-57006b433a07", "f3d5bcce-80e1-44ba-95a0-983d9ddd28f9", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ef1ecdf8-5dd4-43c3-a4c8-d065cb43c745", "c346e877-8705-4b53-ab32-a6d8f9d06bb7", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "628bb844-ea83-4661-bce1-57006b433a07");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef1ecdf8-5dd4-43c3-a4c8-d065cb43c745");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4ebe7abc-54c5-4bf9-ac0b-7f7def2fe2a4", "6e6df423-f463-488e-99e3-b042fe4b2a7a", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0007f4df-8861-4993-bcd6-6f9ddb4be848", "9a9fa689-2f5f-42e2-ae0a-4ed0f572ed3e", "Admin", "ADMIN" });
        }
    }
}
