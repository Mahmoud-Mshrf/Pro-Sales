using System.ComponentModel.DataAnnotations;

namespace CRM.Dtos
{
    public class RegisterDto
    {


        [Required, StringLength(100)]
        public string FirstName { get; set; }
        [Required, StringLength(100)]
        public string LastName { get; set; }

        [Required, StringLength(50)]
        public string UserName { get; set; } 
        [Required, StringLength(128), EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(256)]
        public string Password { get; set; }
        [Required, StringLength(256)]
        public string ConfirmPassword { get; set; }
    }
}
