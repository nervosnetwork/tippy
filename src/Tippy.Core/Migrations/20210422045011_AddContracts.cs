using Microsoft.EntityFrameworkCore.Migrations;

namespace Tippy.Core.Migrations
{
    public partial class AddContracts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       
            migrationBuilder.CreateTable(
               
                name: "Contracts",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false),
                    chainid = table.Column<string>(type: "INTEGER", nullable: false),
                    filename = table.Column<string>(type: "TEXT", nullable: true),
                    filepath = table.Column<int>(type: "TEXT", nullable: false),
                    createtime = table.Column<int>(type: "DATETIME", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
              name: "Contracts");
        }
    }
}
