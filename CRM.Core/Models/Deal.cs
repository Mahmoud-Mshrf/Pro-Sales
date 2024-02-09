using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Models
{
  public class Deal
    {
        [Key]
        public int DealId { get; set; }
        public DateTime DealDate { get; set; }
        public String? description { get; set; }
        public double Price { get; set; }
        public ICollection<Interest> interests { get; set; }=new List<Interest>();
        public Customer?Customer { get; set; }
        [ForeignKey("SalesRepresntativeId")]
        public ApplicationUser? SalesRepresntative { get; set; }
        public Interest? Interest { get; set; }  







    }
}
