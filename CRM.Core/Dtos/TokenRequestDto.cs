using CRM.Core.Custom_Attributes;
using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Dtos
{
    public class TokenRequestDto
    {
        [ValidationErrorOrder(1)]
        [Required, EmailAddress]
        public string Email { get; set; }
        [ValidationErrorOrder(2)]
        [Required]
        public string Password { get; set; }
    }
}
