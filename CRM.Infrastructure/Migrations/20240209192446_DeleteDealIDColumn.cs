using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteDealIDColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
           name: "FK_Interests_Deals_DealId",
           table: "Interests");

            // Drop the index on DealId if it exists
            migrationBuilder.DropIndex(
                name: "IX_Interests_DealId",
                table: "Interests");

            // Drop the DealId column
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
        nullable: false,
        defaultValue: 0);

            // Recreate the foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Interests_Deals_DealId",
                table: "Interests",
                column: "DealId",
                principalTable: "Deals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade); // Adjust the ReferentialAction according to your requirements

            // Recreate the index on DealId if needed
            migrationBuilder.CreateIndex(
                name: "IX_Interests_DealId",
                table: "Interests",
                column: "DealId");
        }
    }
}
