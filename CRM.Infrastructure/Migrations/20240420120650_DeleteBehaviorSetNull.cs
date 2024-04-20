using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBehaviorSetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calls_AspNetUsers_SalesRepresntativeId",
                table: "Calls");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_MarketingModeratorId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_AspNetUsers_SalesRepresntativeId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AspNetUsers_SalesRepresntativeId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SalesRepresntativeId",
                table: "Messages");

            migrationBuilder.AddForeignKey(
                name: "FK_Calls_AspNetUsers_SalesRepresntativeId",
                table: "Calls",
                column: "SalesRepresntativeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_MarketingModeratorId",
                table: "Customers",
                column: "MarketingModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_AspNetUsers_SalesRepresntativeId",
                table: "Deals",
                column: "SalesRepresntativeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AspNetUsers_SalesRepresntativeId",
                table: "Meetings",
                column: "SalesRepresntativeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SalesRepresntativeId",
                table: "Messages",
                column: "SalesRepresntativeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calls_AspNetUsers_SalesRepresntativeId",
                table: "Calls");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_MarketingModeratorId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_AspNetUsers_SalesRepresntativeId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_AspNetUsers_SalesRepresntativeId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SalesRepresntativeId",
                table: "Messages");

            migrationBuilder.AddForeignKey(
                name: "FK_Calls_AspNetUsers_SalesRepresntativeId",
                table: "Calls",
                column: "SalesRepresntativeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_MarketingModeratorId",
                table: "Customers",
                column: "MarketingModeratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_AspNetUsers_SalesRepresntativeId",
                table: "Deals",
                column: "SalesRepresntativeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_AspNetUsers_SalesRepresntativeId",
                table: "Meetings",
                column: "SalesRepresntativeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SalesRepresntativeId",
                table: "Messages",
                column: "SalesRepresntativeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
