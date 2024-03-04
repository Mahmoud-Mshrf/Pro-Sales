using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class EmailDto
    {
        [Required]
        public string Email { get; set; }
    }
}
