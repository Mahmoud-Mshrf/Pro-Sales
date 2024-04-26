using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Ocsp;
using CRM.Core.Dtos;

namespace CRM.Core.Models
{

    public class Meeting
    {
        [Key]
        public string MeetingID { get; set; }
        public DateTime?MeetingDate { get; set; }


        [MaxLength(1000)]
        public string MeetingSummary { get; set; }  
        public DateTime? FollowUpDate { get; set; }
        public bool?connectionState { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("SalesRepresntativeId")]
        public ApplicationUser SalesRepresntative { get; set; }

        public Meeting()
        {

            MeetingID = Guid.NewGuid().ToString();


        }




    }
}
