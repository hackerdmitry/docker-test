using Microsoft.EntityFrameworkCore.Migrations;

namespace DockerTest.Data.Migrations
{
    public partial class ProgressLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountStep",
                table: "Links",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStep",
                table: "Links",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "Links",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tact",
                table: "Links",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountStep",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "CurrentStep",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "Tact",
                table: "Links");
        }
    }
}
