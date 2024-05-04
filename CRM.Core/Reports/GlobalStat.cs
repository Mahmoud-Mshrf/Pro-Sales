using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Reports
{
    public class GlobalStat
    {
        public ItemStat Customers { get; set; }
        public ItemStat Actions { get; set; }
        public ItemStat Deals { get; set; }
        public ItemStat Revenue { get; set; }

    }
    public class ItemStat
    {
        public int Total { get; set; }
        public int ThisWeek { get; set; }
    }
}
