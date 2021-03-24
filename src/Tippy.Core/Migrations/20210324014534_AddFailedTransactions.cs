using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tippy.Core.Migrations
{
    public partial class AddFailedTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FailedTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RawTransaction = table.Column<string>(type: "TEXT", nullable: false),
                    Error = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailedTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FailedTransaction_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FailedTransaction_ProjectId",
                table: "FailedTransaction",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FailedTransaction");
        }
    }
}
