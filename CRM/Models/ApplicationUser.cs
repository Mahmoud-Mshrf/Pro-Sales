using CRM.Helpers;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }

    }
}
