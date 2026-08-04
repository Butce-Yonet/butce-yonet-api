using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ButceYonet.Application.Migrations
{
    /// <inheritdoc />
    public partial class NotebookLabelToUserLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TransactionV2_Amount",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionV2_BankAccountId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionV2_CurrencyId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionV2_Description",
                table: "Transactions",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionV2_ExternalId",
                table: "Transactions",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TransactionV2_IsMatched",
                table: "Transactions",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TransactionV2_IsProceed",
                table: "Transactions",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionV2_Name",
                table: "Transactions",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionV2_NotebookId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionV2_TransactionDate",
                table: "Transactions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionV2_TransactionType",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserLabels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    ColorCode = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLabels", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CategorizedTransactionReportV2",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    NotebookId = table.Column<int>(type: "int", nullable: false),
                    UserLabelId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Term = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorizedTransactionReportV2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategorizedTransactionReportV2_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategorizedTransactionReportV2_Notebooks_NotebookId",
                        column: x => x.NotebookId,
                        principalTable: "Notebooks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategorizedTransactionReportV2_UserLabels_UserLabelId",
                        column: x => x.UserLabelId,
                        principalTable: "UserLabels",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotebookLabelToUserLabels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    NotebookLabelId = table.Column<int>(type: "int", nullable: false),
                    UserLabelId = table.Column<int>(type: "int", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotebookLabelToUserLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotebookLabelToUserLabels_NotebookLabels_NotebookLabelId",
                        column: x => x.NotebookLabelId,
                        principalTable: "NotebookLabels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotebookLabelToUserLabels_UserLabels_UserLabelId",
                        column: x => x.UserLabelId,
                        principalTable: "UserLabels",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransactionLabelV2s",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserLabelId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLabelV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionLabelV2s_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransactionLabelV2s_UserLabels_UserLabelId",
                        column: x => x.UserLabelId,
                        principalTable: "UserLabels",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionV2_BankAccountId",
                table: "Transactions",
                column: "TransactionV2_BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionV2_CurrencyId",
                table: "Transactions",
                column: "TransactionV2_CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionV2_NotebookId",
                table: "Transactions",
                column: "TransactionV2_NotebookId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReportV2_CurrencyId",
                table: "CategorizedTransactionReportV2",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReportV2_NotebookId",
                table: "CategorizedTransactionReportV2",
                column: "NotebookId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReportV2_UserLabelId",
                table: "CategorizedTransactionReportV2",
                column: "UserLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_NotebookLabelToUserLabels_NotebookLabelId",
                table: "NotebookLabelToUserLabels",
                column: "NotebookLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_NotebookLabelToUserLabels_UserLabelId",
                table: "NotebookLabelToUserLabels",
                column: "UserLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLabelV2s_TransactionId",
                table: "TransactionLabelV2s",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLabelV2s_UserLabelId",
                table: "TransactionLabelV2s",
                column: "UserLabelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_BankAccounts_TransactionV2_BankAccountId",
                table: "Transactions",
                column: "TransactionV2_BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Currencies_TransactionV2_CurrencyId",
                table: "Transactions",
                column: "TransactionV2_CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Notebooks_TransactionV2_NotebookId",
                table: "Transactions",
                column: "TransactionV2_NotebookId",
                principalTable: "Notebooks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_BankAccounts_TransactionV2_BankAccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Currencies_TransactionV2_CurrencyId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Notebooks_TransactionV2_NotebookId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "CategorizedTransactionReportV2");

            migrationBuilder.DropTable(
                name: "NotebookLabelToUserLabels");

            migrationBuilder.DropTable(
                name: "TransactionLabelV2s");

            migrationBuilder.DropTable(
                name: "UserLabels");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionV2_BankAccountId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionV2_CurrencyId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionV2_NotebookId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_Amount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_BankAccountId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_CurrencyId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_Description",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_ExternalId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_IsMatched",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_IsProceed",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_Name",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_NotebookId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_TransactionDate",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionV2_TransactionType",
                table: "Transactions");
        }
    }
}
