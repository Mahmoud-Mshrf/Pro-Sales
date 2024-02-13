using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class UpdateNameDto
    {
        [Required,MaxLength(56)]
        public string FirstName { get; set; }
        [Required,MaxLength(56)]
        public string LastName { get; set; }
    }
}
