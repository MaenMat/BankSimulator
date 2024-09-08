using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankSimulator.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAccount24090713322648 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppAccountCustomerInfoFile",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerInfoFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAccountCustomerInfoFile", x => new { x.AccountId, x.CustomerInfoFileId });
                    table.ForeignKey(
                        name: "FK_AppAccountCustomerInfoFile_AppAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AppAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppAccountCustomerInfoFile_AppCustomerInfoFiles_CustomerInfoFileId",
                        column: x => x.CustomerInfoFileId,
                        principalTable: "AppCustomerInfoFiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAccountCustomerInfoFile_AccountId_CustomerInfoFileId",
                table: "AppAccountCustomerInfoFile",
                columns: new[] { "AccountId", "CustomerInfoFileId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppAccountCustomerInfoFile_CustomerInfoFileId",
                table: "AppAccountCustomerInfoFile",
                column: "CustomerInfoFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppAccountCustomerInfoFile");
        }
    }
}
