using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CRM.Core.Services.Implementations.FilterService;

namespace CRM.Core.Reports
{
    public class DailyReport
    {
        public int ActionsCount { get; set; }
        public PagesDto<SalesReport> SalesReports { get; set; }
    }
    public class SalesReport
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Customers { get; set; }
        public int Messages { get; set; }
        public int Deals { get; set; }
        public Meetings Meetings { get; set; } = new Meetings();
        public Calls Calls { get; set; } = new Calls();
    }
    public class Calls
    {
        public int Completed { get; set; }
        public int Missed { get; set; }
        public int Cancelled { get; set; }
        public int Busy { get; set; }
        public int Failed { get; set; }
    }
    public class Meetings
    {
        public int Online { get; set; }
        public int Offline { get; set; }
    }
}
