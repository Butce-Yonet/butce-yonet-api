using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ButceYonet.Application.Migrations
{
    /// <inheritdoc />
    public partial class TransactionLabelV2sRenamedToTransactionLabelsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLabelV2s_Transactions_TransactionId",
                table: "TransactionLabelV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLabelV2s_UserLabels_UserLabelId",
                table: "TransactionLabelV2s");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionLabelV2s",
                table: "TransactionLabelV2s");

            migrationBuilder.RenameTable(
                name: "TransactionLabelV2s",
                newName: "TransactionLabelsV2");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionLabelV2s_UserLabelId",
                table: "TransactionLabelsV2",
                newName: "IX_TransactionLabelsV2_UserLabelId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionLabelV2s_TransactionId",
                table: "TransactionLabelsV2",
                newName: "IX_TransactionLabelsV2_TransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionLabelsV2",
                table: "TransactionLabelsV2",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLabelsV2_Transactions_TransactionId",
                table: "TransactionLabelsV2",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLabelsV2_UserLabels_UserLabelId",
                table: "TransactionLabelsV2",
                column: "UserLabelId",
                principalTable: "UserLabels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLabelsV2_Transactions_TransactionId",
                table: "TransactionLabelsV2");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionLabelsV2_UserLabels_UserLabelId",
                table: "TransactionLabelsV2");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionLabelsV2",
                table: "TransactionLabelsV2");

            migrationBuilder.RenameTable(
                name: "TransactionLabelsV2",
                newName: "TransactionLabelV2s");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionLabelsV2_UserLabelId",
                table: "TransactionLabelV2s",
                newName: "IX_TransactionLabelV2s_UserLabelId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionLabelsV2_TransactionId",
                table: "TransactionLabelV2s",
                newName: "IX_TransactionLabelV2s_TransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionLabelV2s",
                table: "TransactionLabelV2s",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLabelV2s_Transactions_TransactionId",
                table: "TransactionLabelV2s",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionLabelV2s_UserLabels_UserLabelId",
                table: "TransactionLabelV2s",
                column: "UserLabelId",
                principalTable: "UserLabels",
                principalColumn: "Id");
        }
    }
}
