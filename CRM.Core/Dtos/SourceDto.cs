using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class SourceDto
    {
        [Required]
        public int SourceId { get; set; }
        [Required]
        public string SourceName { get; set; }
    }
}
