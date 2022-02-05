using Microsoft.EntityFrameworkCore.Migrations;

namespace Tippy.Core.Migrations
{
    public partial class AddInitCapacityToProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "InitCapacity",
                table: "Projects",
                type: "int",
                nullable: true,
                defaultValue: "2000000000000000000");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "InitCapacity",
               table: "Projects");
        }
    }
}
