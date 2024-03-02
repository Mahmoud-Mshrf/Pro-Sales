using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class MessageDto
    {
        [Required]
        public DateTime MessageDate { get; set; }
        public DateTime FollowUpDate { get; set; }
      
        [MaxLength(500)]
        [Required]
        public string MessageContent { get; set; }
        [Required]
        public int CustomerId { get; set; }

    }
}
