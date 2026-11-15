using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ButceYonet.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "EngagementSignals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserLabelId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    PeriodStartDay = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreateUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    ModifyUser = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budgets_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Budgets_UserLabels_UserLabelId",
                        column: x => x.UserLabelId,
                        principalTable: "UserLabels",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EngagementSignals_BudgetId_Type_OccurredAt",
                table: "EngagementSignals",
                columns: new[] { "BudgetId", "Type", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_CurrencyId",
                table: "Budgets",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId_CurrencyId_UserLabelId",
                table: "Budgets",
                columns: new[] { "UserId", "CurrencyId", "UserLabelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserLabelId",
                table: "Budgets",
                column: "UserLabelId");

            migrationBuilder.AddForeignKey(
                name: "FK_EngagementSignals_Budgets_BudgetId",
                table: "EngagementSignals",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EngagementSignals_Budgets_BudgetId",
                table: "EngagementSignals");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_EngagementSignals_BudgetId_Type_OccurredAt",
                table: "EngagementSignals");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "EngagementSignals");
        }
    }
}
