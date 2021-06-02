using Microsoft.EntityFrameworkCore.Migrations;

namespace POSUNO.Api.Migrations
{
    public partial class Nuevocampo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirsName",
                table: "Customers",
                newName: "FirtsName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirtsName",
                table: "Customers",
                newName: "FirsName");
        }
    }
}
