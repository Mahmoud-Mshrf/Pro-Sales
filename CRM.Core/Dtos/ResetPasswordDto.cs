using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Dtos
{
    public class ResetPasswordDto
    {
        [Required,EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string Password { get; set; }
        [Required,Compare("Password",ErrorMessage ="Password doesn't match with ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}
