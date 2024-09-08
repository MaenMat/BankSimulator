using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankSimulator.Migrations
{
    /// <inheritdoc />
    public partial class AddedTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DestinationAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTransactions_AppAccounts_DestinationAccountId",
                        column: x => x.DestinationAccountId,
                        principalTable: "AppAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppTransactions_AppAccounts_SourceAccountId",
                        column: x => x.SourceAccountId,
                        principalTable: "AppAccounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTransactions_DestinationAccountId",
                table: "AppTransactions",
                column: "DestinationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTransactions_SourceAccountId",
                table: "AppTransactions",
                column: "SourceAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTransactions");
        }
    }
}
