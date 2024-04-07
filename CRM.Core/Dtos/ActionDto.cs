using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class ActionDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Status { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Online { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? Price { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InterestDto Interest { get; set; }
        public string Summary { get; set; }  
        public DateTime Date { get; set; }
        public DateTime? FollowUp { get; set; }
       

    }
}
