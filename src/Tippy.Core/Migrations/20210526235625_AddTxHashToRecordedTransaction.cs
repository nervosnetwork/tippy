using Microsoft.EntityFrameworkCore.Migrations;

namespace Tippy.Core.Migrations
{
    public partial class AddTxHashToRecordedTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TxHash",
                table: "RecordedTransactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TxHash",
                table: "RecordedTransactions");
        }
    }
}
