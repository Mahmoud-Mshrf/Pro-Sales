using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class BusinessDto
    {
        [Required]
        public string CompanyName { get; set; }=string.Empty;
        public string Description { get; set; }=string.Empty;
    }
}
