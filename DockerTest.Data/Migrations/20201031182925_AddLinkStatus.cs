using Microsoft.EntityFrameworkCore.Migrations;

namespace DockerTest.Data.Migrations
{
    public partial class AddLinkStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LinkStatus",
                table: "Links",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkStatus",
                table: "Links");
        }
    }
}
