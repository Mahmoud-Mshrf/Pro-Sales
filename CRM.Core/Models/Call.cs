using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Models
{
    public class Call
    {
        [Key] 
        public int CallID { get; set; }
        public DateOnly CallDate { get; set; }

        public TimeSpan CallTime { get; set; }

        public TimeSpan FollowUpTime { get; set; }

 
        public DateOnly FollowUpDate { get; set; }

        [MaxLength(500)] 
        public string? CallSummery { get; set; }

        [ForeignKey("CustomerId")] 
        public Customer? Customer { get; set; }

        [ForeignKey("SalesRepresntativeId")]
        public ApplicationUser? SalesRepresntative { get; set; }
    }
}