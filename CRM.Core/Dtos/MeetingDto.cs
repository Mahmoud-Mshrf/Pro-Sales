using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class MeetingDto
    {
        [Required]     
        
        public DateTime MeetingDate { get; set; }

        [MaxLength(1000),Required]

        public string MeetingSummary { get; set; }
        public DateTime FollowUpDate { get; set; }

        [Required]
        public ConnectionState connectionState { get; set; }

        [Required]
        public int CustomerId { get; set; }

        
    }
}
