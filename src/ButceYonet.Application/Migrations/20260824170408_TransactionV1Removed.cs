using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ButceYonet.Application.Migrations
{
    /// <inheritdoc />
    public partial class TransactionV1Removed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorizedTransactionReport_NotebookLabels_NotebookLabelId",
                table: "CategorizedTransactionReport");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLabelsV2_Transactions_TransactionId",
                table: "TransactionLabelsV2");

            migrationBuilder.DropTable(
                name: "NotebookLabelToUserLabels");

            migrationBuilder.DropTable(
                name: "TransactionLabels");

            migrationBuilder.DropTable(
                name: "NotebookLabels");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLabelsV2_TransactionId",
                table: "TransactionLabelsV2");

            migrationBuilder.DropIndex(
                name: "IX_CategorizedTransactionReport_NotebookLabelId",
                table: "CategorizedTransactionReport");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "TransactionLabelsV2");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionV2Id",
                table: "TransactionLabelsV2",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserLabelId",
                table: "CategorizedTransactionReport",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReport_UserLabelId",
                table: "CategorizedTransactionReport",
                column: "UserLabelId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorizedTransactionReport_UserLabels_UserLabelId",
                table: "CategorizedTransactionReport",
                column: "UserLabelId",
                principalTable: "UserLabels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorizedTransactionReport_UserLabels_UserLabelId",
                table: "CategorizedTransactionReport");

            migrationBuilder.DropIndex(
                name: "IX_CategorizedTransactionReport_UserLabelId",
                table: "CategorizedTransactionReport");

            migrationBuilder.DropColumn(
                name: "UserLabelId",
                table: "CategorizedTransactionReport");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionV2Id",
                table: "TransactionLabelsV2",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "TransactionLabelsV2",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NotebookLabels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    NotebookId = table.Column<int>(type: "int", nullable: false),
                    ColorCode = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotebookLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotebookLabels_Notebooks_NotebookId",
                        column: x => x.NotebookId,
                        principalTable: "Notebooks",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    BankAccountId = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    NotebookId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    ExternalId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsMatched = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsProceed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Notebooks_NotebookId",
                        column: x => x.NotebookId,
                        principalTable: "Notebooks",
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
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
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
                name: "TransactionLabels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    NotebookLabelId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLabels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionLabels_NotebookLabels_NotebookLabelId",
                        column: x => x.NotebookLabelId,
                        principalTable: "NotebookLabels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransactionLabels_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLabelsV2_TransactionId",
                table: "TransactionLabelsV2",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReport_NotebookLabelId",
                table: "CategorizedTransactionReport",
                column: "NotebookLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_NotebookLabels_NotebookId",
                table: "NotebookLabels",
                column: "NotebookId");

            migrationBuilder.CreateIndex(
                name: "IX_NotebookLabelToUserLabels_NotebookLabelId",
                table: "NotebookLabelToUserLabels",
                column: "NotebookLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_NotebookLabelToUserLabels_UserLabelId",
                table: "NotebookLabelToUserLabels",
                column: "UserLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLabels_NotebookLabelId",
                table: "TransactionLabels",
                column: "NotebookLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLabels_TransactionId",
                table: "TransactionLabels",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BankAccountId",
                table: "Transactions",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CurrencyId",
                table: "Transactions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_NotebookId",
                table: "Transactions",
                column: "NotebookId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorizedTransactionReport_NotebookLabels_NotebookLabelId",
                table: "CategorizedTransactionReport",
                column: "NotebookLabelId",
                principalTable: "NotebookLabels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLabelsV2_Transactions_TransactionId",
                table: "TransactionLabelsV2",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }
    }
}
