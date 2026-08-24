using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ButceYonet.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddNotebookV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotebookId",
                table: "RecurringTransactions",
                newName: "UserId");

            // RenameColumn az önce eski NotebookId değerlerini olduğu gibi UserId kolonuna taşıdı;
            // bu değerler aslında bir NotebookId, gerçek UserId değil. NotebookUsers (IsDefault=1)
            // üzerinden gerçek sahip kullanıcıya eşle. NotebookUsers henüz silinmediği için bu join çalışır.
            migrationBuilder.Sql(@"
                UPDATE RecurringTransactions rt
                JOIN NotebookUsers nu ON nu.NotebookId = rt.UserId AND nu.IsDefault = 1
                SET rt.UserId = nu.UserId;
            ");

            migrationBuilder.AddColumn<int>(
                name: "NotebookV2Id",
                table: "TransactionsV2",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NotebookId",
                table: "NonCategorizedTransactionReport",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "NotebookV2Id",
                table: "NonCategorizedTransactionReport",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NotebookId",
                table: "CategorizedTransactionReportV2",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "NotebookV2Id",
                table: "CategorizedTransactionReportV2",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotebookV2",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    TermStart = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TermEnd = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotebookV2", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsV2_NotebookV2Id",
                table: "TransactionsV2",
                column: "NotebookV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_NonCategorizedTransactionReport_NotebookV2Id",
                table: "NonCategorizedTransactionReport",
                column: "NotebookV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_CategorizedTransactionReportV2_NotebookV2Id",
                table: "CategorizedTransactionReportV2",
                column: "NotebookV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_NotebookV2_UserId",
                table: "NotebookV2",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotebookV2_UserId_TermStart",
                table: "NotebookV2",
                columns: new[] { "UserId", "TermStart" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategorizedTransactionReportV2_NotebookV2_NotebookV2Id",
                table: "CategorizedTransactionReportV2",
                column: "NotebookV2Id",
                principalTable: "NotebookV2",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NonCategorizedTransactionReport_NotebookV2_NotebookV2Id",
                table: "NonCategorizedTransactionReport",
                column: "NotebookV2Id",
                principalTable: "NotebookV2",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionsV2_NotebookV2_NotebookV2Id",
                table: "TransactionsV2",
                column: "NotebookV2Id",
                principalTable: "NotebookV2",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorizedTransactionReportV2_NotebookV2_NotebookV2Id",
                table: "CategorizedTransactionReportV2");

            migrationBuilder.DropForeignKey(
                name: "FK_NonCategorizedTransactionReport_NotebookV2_NotebookV2Id",
                table: "NonCategorizedTransactionReport");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionsV2_NotebookV2_NotebookV2Id",
                table: "TransactionsV2");

            migrationBuilder.DropTable(
                name: "NotebookV2");

            migrationBuilder.DropIndex(
                name: "IX_TransactionsV2_NotebookV2Id",
                table: "TransactionsV2");

            migrationBuilder.DropIndex(
                name: "IX_NonCategorizedTransactionReport_NotebookV2Id",
                table: "NonCategorizedTransactionReport");

            migrationBuilder.DropIndex(
                name: "IX_CategorizedTransactionReportV2_NotebookV2Id",
                table: "CategorizedTransactionReportV2");

            migrationBuilder.DropColumn(
                name: "NotebookV2Id",
                table: "TransactionsV2");

            migrationBuilder.DropColumn(
                name: "NotebookV2Id",
                table: "NonCategorizedTransactionReport");

            migrationBuilder.DropColumn(
                name: "NotebookV2Id",
                table: "CategorizedTransactionReportV2");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RecurringTransactions",
                newName: "NotebookId");

            migrationBuilder.AlterColumn<int>(
                name: "NotebookId",
                table: "NonCategorizedTransactionReport",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NotebookId",
                table: "CategorizedTransactionReportV2",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
