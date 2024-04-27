using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CRM.Core.Dtos;

namespace CRM.Core.Models
{

    public class Message
    {
        [Key]
        public string MessageID { get; set; }

        public DateTime MessageDate { get; set; }
        [Required]
        [MaxLength(1000)]
        public string MessageContent { get; set; }
        public DateTime? FollowUpDate { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("SalesRepresntativeId")]
        public ApplicationUser SalesRepresntative { get; set; }
        public Message()
        {
            MessageID = Guid.NewGuid().ToString();

        }

    }
}