using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interests_Deals_DealId",
                table: "Interests");

            migrationBuilder.DropIndex(
                name: "IX_Interests_DealId",
                table: "Interests");

            migrationBuilder.DropColumn(
                name: "DealId",
                table: "Interests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DealId",
                table: "Interests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interests_DealId",
                table: "Interests",
                column: "DealId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interests_Deals_DealId",
                table: "Interests",
                column: "DealId",
                principalTable: "Deals",
                principalColumn: "DealId");
        }
    }
}
