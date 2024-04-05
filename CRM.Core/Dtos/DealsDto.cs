using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class DealsDto
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
       
        public string id { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ActionType type { get; set; }
        [Required]
        public double price { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]

        public InterestDto interest { get; set; }
        [Required]
        public String summary { get; set; }
        [Required]
        public DateTime date { get; set; }

        

        [Required]

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int CustomerId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int InterestId { get; set; }

        





    }
}
