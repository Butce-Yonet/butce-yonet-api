using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ButceYonet.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddEngagementSignalIsSent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSent",
                table: "EngagementSignals",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "EngagementSignals",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EngagementSignals_IsSent_OccurredAt",
                table: "EngagementSignals",
                columns: new[] { "IsSent", "OccurredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EngagementSignals_IsSent_OccurredAt",
                table: "EngagementSignals");

            migrationBuilder.DropColumn(
                name: "IsSent",
                table: "EngagementSignals");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "EngagementSignals");
        }
    }
}
