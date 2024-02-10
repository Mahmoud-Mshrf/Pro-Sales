using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Models
{
    public enum Gender
    {
        Male
        , Female
    }
    public class Customer
    {
      
       
            [Key] 
            public int CustomerId { get; set; }

            [Required] 
            [MaxLength(50)] 
            public string? FirstName { get; set; }

            [Required]
            [MaxLength(50)]
            public string? LastName { get; set; }

            [Phone] 
            public string? Phone { get; set; }

            [EmailAddress] 
            public string? Email { get; set; }

            [Range(18, 100)] 
            public int Age { get; set; }

            [EnumDataType(typeof(Gender))] 
            public Gender Gender { get; set; }

            [MaxLength(50)]
            public String? City { get; set; }

            public DateTime AdditionDate { get; set; }

            [ForeignKey("SalesRepresntativeId")] 
            public ApplicationUser? SalesRepresntative { get; set; }

            [ForeignKey("MarketingModeratorId")]
            public ApplicationUser? MarketingModerator { get; set; }

            [ForeignKey("SourceId")]
            public Source? Source { get; set; }

            public ICollection<Interest> Interests { get; set; } = new List<Interest>();

            public ICollection<Message> Messages { get; set; } = new List<Message>();

            public ICollection<Call> Calls { get; set; } = new List<Call>();

            public ICollection <Deal> Deals {  get; set; } = new List<Deal>();
            public ICollection<Meeting> Meetings { get; set; }=new List<Meeting>();

        
    
    }
}
