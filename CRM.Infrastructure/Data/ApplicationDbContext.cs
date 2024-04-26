using CRM.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CRM.Infrastructure.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
        }
        override protected void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<ApplicationUser>().ToTable("Users");
            //builder.Entity<IdentityRole>().ToTable("Roles");
            //builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<Customer>()
            .HasMany(c => c.Interests)
            .WithMany(i => i.Customers)
            .UsingEntity(j => j.ToTable("CustomerInterest"));
            builder.Entity<Meeting>()
            .Property(m => m.FollowUpDate)
            .HasDefaultValue(null);
            builder.Entity<Call>()
           .Property(m => m.FollowUpDate)
           .HasDefaultValue(null);
            builder.Entity<Message>()
           .Property(m => m.FollowUpDate)
           .HasDefaultValue(null);

        }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Call> Calls { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Business> Business { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<Source> Sources { get; set; }  
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Deal> Deals { get; set; }  



    }
}
