using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;


namespace CRM.Core.Dtos
{
    public class CallDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string id { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ActionType type { get; set; }

        [EnumDataType(typeof(CallStatus))]
        public CallStatus status { get; set; }
        [MaxLength(500)]
        public string summary { get; set; }
        public DateTime date { get; set; }
        public DateTime followUp { get; set; }

        [Required]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int CustomerId { get; set; }


    }
}
