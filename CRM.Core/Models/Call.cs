using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CRM.Core.Dtos;

namespace CRM.Core.Models
{
    public enum CallStatus
    {
      
        Completed,         //The call has been successfully completed.
        Missed,           //The call was not answered.
        Cancelled,       //The call was intentionally terminated before completion, either by the caller or the recipient.
        Busy,           //The recipient's line was busy when the call was attempted.
        Failed,        //The call attempt was unsuccessful due to technical reasons or other issues.
      
    }

    public class Call
    {
        [Key]
        public string CallID { get; set; }
        public DateTime CallDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public CallStatus? CallStatus { get; set; }


        [MaxLength(500)]
        public string CallSummery { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("SalesRepresntativeId")]
        public ApplicationUser SalesRepresntative { get; set; }


        public Call()
        {
            CallID = Guid.NewGuid().ToString();

        }
    }
}
