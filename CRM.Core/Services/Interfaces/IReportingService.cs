using CRM.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Interfaces
{
    public interface IReportingService
    {
        Task<DailyReport> MainReport(int page, int size, string within);
        Task<GlobalStat> GlobalStatAsync();
        Task<SalesRepresentativeReport> SalesReport(string salesId, string within);
        Task<IEnumerable<ItemCount>> DealsReport(string within);
        Task<IEnumerable<ItemCount>> SourcesReport(string within);
    }
}
