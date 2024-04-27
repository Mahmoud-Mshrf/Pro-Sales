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
        Task<DailyReport> GetDailyReport(int page, int size);
    }
}
