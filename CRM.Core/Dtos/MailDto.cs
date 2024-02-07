using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Dtos
{
    public class MailDto
    {
        [Required]
        public string MailTo { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Content { get; set; }
        public IList<IFormFile> Attachments { get; set; }
    }
}
