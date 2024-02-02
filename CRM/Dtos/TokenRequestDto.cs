using System.ComponentModel.DataAnnotations;

namespace CRM.Dtos
{
    public class TokenRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
