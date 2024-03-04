using CRM.Core.Custom_Attributes;
using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Dtos
{
    public class TokenRequestDto
    {
        [Required(ErrorMessage = "Enter your username or email.")]
        [ValidationErrorOrder(1)]
        public string LoginIdentifier { get; set; }
        [ValidationErrorOrder(2)]
        [Required]
        public string Password { get; set; }
    }
}
