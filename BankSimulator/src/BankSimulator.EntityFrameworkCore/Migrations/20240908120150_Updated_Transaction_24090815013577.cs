using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankSimulator.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTransaction24090815013577 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionStatus",
                table: "AppTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionStatus",
                table: "AppTransactions");
        }
    }
}
