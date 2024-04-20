using CRM.Core.Helpers;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public IEnumerable<Call> Calls { get; set; }
        public IEnumerable<Deal> Deals { get; set; }
        public IEnumerable<Meeting> meetings { get; set; }

    }
}
