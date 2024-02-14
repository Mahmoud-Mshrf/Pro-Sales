using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class InterestDto
    {
        [Required]
        public int InterestID { get; set; }
        [Required]
        public string InterestName { get; set; }
    }
}
