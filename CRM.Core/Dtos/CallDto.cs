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
    public class CallDto
    {
        
        public DateTime CallDate { get; set; }
        public DateTime FollowUpDate { get; set; }
        [EnumDataType(typeof(CallStatus))]
        public CallStatus CallStatus { get; set; }

        [MaxLength(500)]
        public string CallSummery { get; set; }
        [Required]
        public int CustomerId { get; set; }
        

        
        
    }
}
