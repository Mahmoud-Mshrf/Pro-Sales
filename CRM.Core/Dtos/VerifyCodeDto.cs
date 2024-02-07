using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Dtos
{
    public class VerifyCodeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
