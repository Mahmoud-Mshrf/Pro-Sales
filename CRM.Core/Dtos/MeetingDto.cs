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
    public class MeetingDto : AddMeetingDto
    {


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string id { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ActionType type { get; set; }


    }
}
