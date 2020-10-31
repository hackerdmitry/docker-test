using Microsoft.EntityFrameworkCore.Migrations;

namespace DockerTest.Data.Migrations
{
    public partial class DoubleTact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Tact",
                table: "Links",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Tact",
                table: "Links",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
