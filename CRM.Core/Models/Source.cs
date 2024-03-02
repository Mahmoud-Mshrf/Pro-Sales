using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Models
{
    public class Source
    {
        [Key] 
        public int SourceId { get; set; }

        [MaxLength(50)]
        public string SourceName { get; set; }

        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
