using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class ReturnCustomerDto : CustomerDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[] Errors { get; set; }
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        //public int CustomerId { get; set; }
        [JsonIgnore]
        public bool IsSuccess { get; set; }
        public UserDto SalesRepresentative { get; set; }
        public int Id { get; set; }
        public DateTime AdditionDate { get; set; }

        public ReturnCustomerDto()
        {
            Errors = null;
        }
    }
}
