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
    public class DealsDto : AddDealDto
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]

        public string id { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ActionType type { get; set; }

        public InterestDto interest { get; set; }

    }
}
