using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class ReturnUserRolesDto : UserRolesDTO
    {
        [JsonIgnore]
        public bool IsSucces { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }
        public ReturnUserRolesDto()
        {
            Errors = null;
        }
    }
}
