using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Models
{
    public class Message
    {
        [Key] 
        public int MessageID { get; set; }
        public DateOnly MessageDate { get; set; }
        public TimeSpan MessageTime { get; set; }
        [Required]
        [MaxLength(1000)]
        public string? MessageContent { get; set; }

        public DateOnly FollowUpDate { get; set; }
        public TimeSpan FollowUpTime { get; set; }
        public string? Replay { get; set; }

        [ForeignKey("CustomerId")] // Specify the foreign key property
        public Customer? Customer { get; set; }

        [ForeignKey("SalesRepresntativeId")]
        public ApplicationUser? SalesRepresntative { get; set; }

    }
}