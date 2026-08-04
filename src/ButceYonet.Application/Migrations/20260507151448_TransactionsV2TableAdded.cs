using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ButceYonet.Application.Migrations
{
    /// <inheritdoc />
    public partial class TransactionsV2TableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionV2Id",
                table: "TransactionLabelsV2",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TransactionsV2",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    NotebookId = table.Column<int>(type: "int", nullable: true),
                    BankAccountId = table.Column<int>(type: "int", nullable: true),
                    ExternalId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    IsMatched = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsProceed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionsV2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionsV2_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransactionsV2_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransactionsV2_Notebooks_NotebookId",
                        column: x => x.NotebookId,
                        principalTable: "Notebooks",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLabelsV2_TransactionV2Id",
                table: "TransactionLabelsV2",
                column: "TransactionV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsV2_BankAccountId",
                table: "TransactionsV2",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsV2_CurrencyId",
                table: "TransactionsV2",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsV2_NotebookId",
                table: "TransactionsV2",
                column: "NotebookId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLabelsV2_TransactionsV2_TransactionV2Id",
                table: "TransactionLabelsV2",
                column: "TransactionV2Id",
                principalTable: "TransactionsV2",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLabelsV2_TransactionsV2_TransactionV2Id",
                table: "TransactionLabelsV2");

            migrationBuilder.DropTable(
                name: "TransactionsV2");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLabelsV2_TransactionV2Id",
                table: "TransactionLabelsV2");

            migrationBuilder.DropColumn(
                name: "TransactionV2Id",
                table: "TransactionLabelsV2");

        }
    }
}
