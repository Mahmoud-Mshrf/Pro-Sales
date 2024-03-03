using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class ReturnInterstsDto
    {
        [JsonIgnore]
        public bool IsSuccess { get; set; }
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }
        public IList<InterestDto> Interests { get; set; }
        public ReturnInterstsDto()
        {
            Errors = null;
        }
    }
}
