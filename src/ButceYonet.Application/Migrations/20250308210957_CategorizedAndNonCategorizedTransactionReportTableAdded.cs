using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ButceYonet.Application.Migrations
{
    /// <inheritdoc />
    public partial class CategorizedAndNonCategorizedTransactionReportTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategorizedTransactionReport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    NotebookId = table.Column<int>(type: "int", nullable: false),
                    NotebookLabelId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Term = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NotebookId1 = table.Column<int>(type: "int", nullable: false),
                    NotebookLabelId1 = table.Column<int>(type: "int", nullable: false),
                    CurrencyId1 = table.Column<int>(type: "int", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorizedTransactionReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategorizedTransactionReport_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategorizedTransactionReport_Currencies_CurrencyId1",
                        column: x => x.CurrencyId1,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategorizedTransactionReport_NotebookLabels_NotebookLabelId",
                        column: x => x.NotebookLabelId,
                        principalTable: "NotebookLabels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategorizedTransactionReport_NotebookLabels_NotebookLabelId1",
                        column: x => x.NotebookLabelId1,
                        principalTable: "NotebookLabels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategorizedTransactionReport_Notebooks_NotebookId",
                        column: x => x.NotebookId,
                        principalTable: "Notebooks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategorizedTransactionReport_Notebooks_NotebookId1",
                        column: x => x.NotebookId1,
                        principalTable: "Notebooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NonCategorizedTransactionReport",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    NotebookId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Term = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CurrencyId1 = table.Column<int>(type: "int", nullable: false),
                    NotebookId1 = table.Column<int>(type: "int", nullable: true),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonCategorizedTransactionReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonCategorizedTransactionReport_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NonCategorizedTransactionReport_Currencies_CurrencyId1",
                        column: x => x.CurrencyId1,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NonCategorizedTransactionReport_Notebooks_NotebookId",
                        column: x => x.NotebookId,
                        principalTable: "Notebooks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NonCategorizedTransactionReport_Notebooks_NotebookId1",
                        column: x => x.NotebookId1,
                        principalTable: "Notebooks",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReport_CurrencyId",
                table: "CategorizedTransactionReport",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReport_CurrencyId1",
                table: "CategorizedTransactionReport",
                column: "CurrencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReport_NotebookId",
                table: "CategorizedTransactionReport",
                column: "NotebookId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReport_NotebookId1",
                table: "CategorizedTransactionReport",
                column: "NotebookId1");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReport_NotebookLabelId",
                table: "CategorizedTransactionReport",
                column: "NotebookLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReport_NotebookLabelId1",
                table: "CategorizedTransactionReport",
                column: "NotebookLabelId1");

            migrationBuilder.CreateIndex(
                name: "IX_NonCategorizedTransactionReport_CurrencyId",
                table: "NonCategorizedTransactionReport",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_NonCategorizedTransactionReport_CurrencyId1",
                table: "NonCategorizedTransactionReport",
                column: "CurrencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_NonCategorizedTransactionReport_NotebookId",
                table: "NonCategorizedTransactionReport",
                column: "NotebookId");

            migrationBuilder.CreateIndex(
                name: "IX_NonCategorizedTransactionReport_NotebookId1",
                table: "NonCategorizedTransactionReport",
                column: "NotebookId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategorizedTransactionReport");

            migrationBuilder.DropTable(
                name: "NonCategorizedTransactionReport");
        }
    }
}
