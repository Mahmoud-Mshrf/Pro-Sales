using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Reports
{
    public class GlobalStat
    {
        public ItemStat Customers { get; set; } = new ItemStat();
        public ItemStat Actions { get; set; }= new ItemStat();
        public ItemStat Deals { get; set; } = new ItemStat();
        public ItemStat Revenue { get; set; } = new ItemStat();

    }
    public class ItemStat
    {
        public int Total { get; set; }
        public int ThisWeek { get; set; }
        public int ThisMonth { get; set; }
        public int Today { get; set; }
    }
}
