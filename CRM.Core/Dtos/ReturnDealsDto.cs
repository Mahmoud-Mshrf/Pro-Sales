using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class ReturnDealsDto
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public List<DealsDto> Deals { get; set; }
    }
}
