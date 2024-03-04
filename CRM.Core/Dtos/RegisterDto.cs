using CRM.Core.Custom_Attributes;
using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Dtos
{
    public class RegisterDto
    {

        [ValidationErrorOrder(1)]
        [Required, StringLength(100)]
        public string FirstName { get; set; }
        [ValidationErrorOrder(2)]
        [Required, StringLength(100)]
        public string LastName { get; set; }
        [ValidationErrorOrder(3)]
        [Required, StringLength(50)]
        public string Username { get; set; }
        [ValidationErrorOrder(4)]
        [Required, StringLength(128), EmailAddress]
        public string Email { get; set; }
        [ValidationErrorOrder(5)]
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$",
        ErrorMessage = "Password must have at least one non-alphanumeric character, one digit, and one uppercase character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [ValidationErrorOrder(6)]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
