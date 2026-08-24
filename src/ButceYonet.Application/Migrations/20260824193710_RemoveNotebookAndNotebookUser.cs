using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ButceYonet.Application.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotebookAndNotebookUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorizedTransactionReportV2_Notebooks_NotebookId",
                table: "CategorizedTransactionReportV2");

            migrationBuilder.DropForeignKey(
                name: "FK_NonCategorizedTransactionReport_Notebooks_NotebookId",
                table: "NonCategorizedTransactionReport");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionsV2_Notebooks_NotebookId",
                table: "TransactionsV2");

            migrationBuilder.DropTable(
                name: "NotebookUsers");

            migrationBuilder.DropTable(
                name: "Notebooks");

            migrationBuilder.DropIndex(
                name: "IX_TransactionsV2_NotebookId",
                table: "TransactionsV2");

            migrationBuilder.DropIndex(
                name: "IX_NonCategorizedTransactionReport_NotebookId",
                table: "NonCategorizedTransactionReport");

            migrationBuilder.DropIndex(
                name: "IX_CategorizedTransactionReportV2_NotebookId",
                table: "CategorizedTransactionReportV2");

            migrationBuilder.DropColumn(
                name: "NotebookId",
                table: "TransactionsV2");

            migrationBuilder.DropColumn(
                name: "NotebookId",
                table: "NonCategorizedTransactionReport");

            migrationBuilder.DropColumn(
                name: "NotebookId",
                table: "CategorizedTransactionReportV2");

            migrationBuilder.AlterColumn<int>(
                name: "NotebookV2Id",
                table: "TransactionsV2",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NotebookV2Id",
                table: "NonCategorizedTransactionReport",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NotebookV2Id",
                table: "CategorizedTransactionReportV2",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NotebookV2Id",
                table: "TransactionsV2",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "NotebookId",
                table: "TransactionsV2",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NotebookV2Id",
                table: "NonCategorizedTransactionReport",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "NotebookId",
                table: "NonCategorizedTransactionReport",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NotebookV2Id",
                table: "CategorizedTransactionReportV2",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "NotebookId",
                table: "CategorizedTransactionReportV2",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notebooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    IsDefault = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notebooks", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotebookUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    NotebookId = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreateUser = table.Column<string>(type: "longtext", nullable: true),
                    IsDefault = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifyUser = table.Column<string>(type: "longtext", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotebookUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotebookUsers_Notebooks_NotebookId",
                        column: x => x.NotebookId,
                        principalTable: "Notebooks",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsV2_NotebookId",
                table: "TransactionsV2",
                column: "NotebookId");

            migrationBuilder.CreateIndex(
                name: "IX_NonCategorizedTransactionReport_NotebookId",
                table: "NonCategorizedTransactionReport",
                column: "NotebookId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReportV2_NotebookId",
                table: "CategorizedTransactionReportV2",
                column: "NotebookId");

            migrationBuilder.CreateIndex(
                name: "IX_NotebookUsers_NotebookId",
                table: "NotebookUsers",
                column: "NotebookId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorizedTransactionReportV2_Notebooks_NotebookId",
                table: "CategorizedTransactionReportV2",
                column: "NotebookId",
                principalTable: "Notebooks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NonCategorizedTransactionReport_Notebooks_NotebookId",
                table: "NonCategorizedTransactionReport",
                column: "NotebookId",
                principalTable: "Notebooks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionsV2_Notebooks_NotebookId",
                table: "TransactionsV2",
                column: "NotebookId",
                principalTable: "Notebooks",
                principalColumn: "Id");
        }
    }
}
