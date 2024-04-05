using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultManager : Migration
    { 
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create DefaultAdmin user
            var hasher = new PasswordHasher<IdentityUser>();
            var hashedPassword = hasher.HashPassword(null, "Admin@123");

            migrationBuilder.Sql($@"INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,AccessFailedCount,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,FirstName,LastName)
                            VALUES ('{Guid.NewGuid().ToString()}', 'DefaultManager', 'DEFAULTMANAGER', 'defaultmanager@gmail.com', 'DEFAULTMANAGER@GMAIL.COM', 1, '{hashedPassword}', 'RandomSecurityStamp', '{Guid.NewGuid().ToString()}',0,0,0,1,'Default','Manager')");

            // Assign DefaultAdmin to Admin role
            migrationBuilder.Sql("INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ((SELECT Id FROM AspNetUsers WHERE UserName = 'DefaultManager'), (SELECT Id FROM AspNetRoles WHERE [Name] = 'Manager'))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove records added in the Up method
            migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE UserName = 'DefaultManager')");
            migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE UserName = 'DefaultManager'");
        }
    }
}
