using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Models
{
    public class Business
    {
        [Key] 
        public int BusinessId { get; set; }

        [Required]
        [MaxLength(50)]
        public string?CompanyName { get; set; }

        [MaxLength(500)]
        public string?Description { get; set; }

        [ForeignKey("ManagerId")] 
        public ApplicationUser?Manager { get; set; }
    }
}
