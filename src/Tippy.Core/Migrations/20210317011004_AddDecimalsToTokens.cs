using Microsoft.EntityFrameworkCore.Migrations;

namespace Tippy.Core.Migrations
{
    public partial class AddDecimalsToTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Decimals",
                table: "Tokens",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Decimals",
                table: "Tokens");
        }
    }
}
