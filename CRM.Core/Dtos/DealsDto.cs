using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class DealsDto
    {
        [Required]
        public DateTime DealDate { get; set; }
        [Required]
        public String description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int InterestId { get; set; }

    }
}
