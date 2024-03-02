using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Models
{
    public enum ConnectionState
    {
        Online,
        Offline
    }
    public class Meeting
    {
        [Key] 
        public int MeetingID { get; set; }        
        public DateTime MeetingDate { get; set; }

        [MaxLength(1000)] 
        public string MeetingSummary { get; set; }  
        public DateTime FollowUpDate { get; set; }
        public ConnectionState connectionState { get; set; }

        [ForeignKey("CustomerId")] 
        public Customer Customer {  get; set; }

        [ForeignKey("SalesRepresntativeId")]
        public ApplicationUser SalesRepresntative { get; set; }

        
    
}
}
