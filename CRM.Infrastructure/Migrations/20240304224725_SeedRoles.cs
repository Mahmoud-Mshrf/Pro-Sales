using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert Admin Role and Moderator Role and Sales_Representative Role in AspNetRoles Table
            migrationBuilder.Sql("INSERT INTO AspNetRoles (Id, [Name], NormalizedName) VALUES ('1', 'Manager', 'MANAGER')");
            migrationBuilder.Sql("INSERT INTO AspNetRoles (Id, [Name], NormalizedName) VALUES ('2', 'Marketing_Moderator', 'MARKETING_MODERATOR')");
            migrationBuilder.Sql("INSERT INTO AspNetRoles (Id, [Name], NormalizedName) VALUES ('3', 'Sales_Representative', 'SALES_REPRESENTATIVE')");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete Admin Role and Moderator Role and Sales_Representative Role in AspNetRoles Table
            migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE Id IN ('1', '2', '3')");
        }
    }
}
