using Microsoft.EntityFrameworkCore.Migrations;

namespace DockerTest.Data.Migrations
{
    public partial class RemoveProgressLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Links");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "Links",
                type: "integer",
                nullable: true);
        }
    }
}
