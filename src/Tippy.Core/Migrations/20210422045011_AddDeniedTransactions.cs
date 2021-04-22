using Microsoft.EntityFrameworkCore.Migrations;

namespace Tippy.Core.Migrations
{
    public partial class AddDeniedTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeniedTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TxHash = table.Column<string>(type: "TEXT", nullable: false),
                    DenyType = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeniedTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeniedTransactions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeniedTransactions_ProjectId",
                table: "DeniedTransactions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DeniedTransactions_TxHash_DenyType",
                table: "DeniedTransactions",
                columns: new[] { "TxHash", "DenyType" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeniedTransactions");
        }
    }
}
