using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace CRM.Core.Dtos
{
    public class MeetingDto
    {


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string id { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ActionType type { get; set; }
        [Required]
        public bool online { get; set; }
        [MaxLength(1000), Required]
        public string summary { get; set; }
        [Required]     
        public DateTime date { get; set; }
        [Required]
        public DateTime followUp { get; set; }
        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int CustomerId { get; set; }

        
    }
}
