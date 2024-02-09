using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Models
{
    public class Meeting
    {
        [Key] 
        public int MeetingID { get; set; }        
        public DateOnly MeetingDate { get; set; }
        public TimeSpan MeetingTime { get; set; }

        [MaxLength(1000)] 
        public string?MeetingSummary { get; set; }  
        public DateOnly FollowUpDate { get; set; }
        public TimeSpan FollowUpTime { get; set; }
        public bool Online { get; set; }

        [ForeignKey("CustomerId")] 
        public Customer ?Customer {  get; set; }

        [ForeignKey("SalesRepresntativeId")]
        public ApplicationUser?SalesRepresntative { get; set; }

        
    
}
}
