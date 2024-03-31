using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class ActionDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Summary { get; set; }
        public DateTime Date { get; set; }
    }
}
