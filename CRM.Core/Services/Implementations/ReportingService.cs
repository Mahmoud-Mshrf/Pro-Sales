using CRM.Core.Models;
using CRM.Core.Reports;
using CRM.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Implementations
{
    public class ReportingService : IReportingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFilterService _filterService;

        public ReportingService(IUnitOfWork unitOfWork, IFilterService filterService)
        {
            _unitOfWork = unitOfWork;
            _filterService = filterService;
        }

        public async Task<DailyReport> GetDailyReport(int page, int size)
        {
            var salesReps = await _unitOfWork.UserManager.GetUsersInRoleAsync("Sales Representative");
            var customers = await _unitOfWork.Customers.GetAllAsync(x => x.AdditionDate.Day == DateTime.UtcNow.Day, ["SalesRepresntative"]);
            var messages = await _unitOfWork.Messages.GetAllAsync(x => x.MessageDate.Day == DateTime.UtcNow.Day, ["SalesRepresntative"]);
            var calls = await _unitOfWork.Calls.GetAllAsync(x => x.CallDate.Day == DateTime.UtcNow.Day, ["SalesRepresntative"]);
            var meetings = await _unitOfWork.Meetings.GetAllAsync(x => x.MeetingDate.Value.Day == DateTime.UtcNow.Day, ["SalesRepresntative"]);
            var deals = await _unitOfWork.Deals.GetAllAsync(x => x.DealDate.Value.Day == DateTime.UtcNow.Day, ["SalesRepresntative"]);

            var salesReports = new List<SalesReport>();

            foreach (var sales in salesReps)
            {
                var salesreport = new SalesReport();
                salesreport.FirstName = sales.FirstName;
                salesreport.LastName = sales.LastName;
                salesreport.Customers = customers.Where(c => c.SalesRepresntative.Id == sales.Id).Count();
                salesreport.Messages = messages.Where(m => m.SalesRepresntative.Id == sales.Id).Count();
                salesreport.Meetings.Online = meetings.Where(m => m.SalesRepresntative.Id == sales.Id && m.connectionState.Value).Count();
                salesreport.Meetings.Offline = meetings.Where(m => m.SalesRepresntative.Id == sales.Id && !m.connectionState.Value).Count();
                salesreport.Deals = deals.Where(d => d.SalesRepresntative.Id == sales.Id).Count();
                salesreport.Calls.Completed = calls.Where(d => d.SalesRepresntative.Id == sales.Id && d.CallStatus == CallStatus.Completed).Count();
                salesreport.Calls.Missed = calls.Where(d => d.SalesRepresntative.Id == sales.Id && d.CallStatus == CallStatus.Missed).Count();
                salesreport.Calls.Cancelled = calls.Where(d => d.SalesRepresntative.Id == sales.Id && d.CallStatus == CallStatus.Cancelled).Count();
                salesreport.Calls.Busy = calls.Where(d => d.SalesRepresntative.Id == sales.Id && d.CallStatus == CallStatus.Busy).Count();
                salesreport.Calls.Failed = calls.Where(d => d.SalesRepresntative.Id == sales.Id && d.CallStatus == CallStatus.Failed).Count();

                salesReports.Add(salesreport);
            }
            var pages = _filterService.Paginate(salesReports, page, size);
            var report = new DailyReport
            {
                ActionsCount = messages.Count() + calls.Count() + deals.Count() + messages.Count(),
                SalesReports = pages
            };
            return report;
        }
        public async Task<GlobalStat> GlobalStatAsync()
        {
            var customers =await _unitOfWork.Customers.GetAllAsync();
            var allCusotmers = customers.Count();
            // return count of this week customers
            var thisWeekCustomers = customers.Where(c => c.AdditionDate >= DateTime.Now.AddDays(-7)).Count();
            
            var deals = await _unitOfWork.Deals.GetAllAsync();
            var allDeals = deals.Count();
            var thisWeekDeals = deals.Where(d => d.DealDate >= DateTime.Now.AddDays(-7));
            var thisWeekDealsCount = thisWeekDeals.Count();

            var revenues = (int) deals.Sum(d => d.Price);
            var thisWeekrevenues =(int) thisWeekDeals.Sum(d => d.Price);

            var meetings = await _unitOfWork.Meetings.GetAllAsync();
            var allMeetings = meetings.Count();
            var thisWeekMeetings = meetings.Where(m=>m.MeetingDate>= DateTime.Now.AddDays(-7)).Count();

            var calls = await _unitOfWork.Calls.GetAllAsync();
            var allCalls = calls.Count();
            var thisWeekCalls = calls.Where(c=>c.CallDate>= DateTime.Now.AddDays(-7)).Count();

            var messages = await _unitOfWork.Messages.GetAllAsync();
            var allMessages = messages.Count();
            var thisWeekMessages = messages.Where(m=>m.MessageDate >= DateTime.Now.AddDays(-7)).Count();

            var allActions = allCalls + allMessages + allMeetings + allDeals;
            var thisWeekActions = thisWeekCalls + thisWeekMessages + thisWeekMeetings + thisWeekDealsCount;
            return new GlobalStat
            {
                Customers =
                {
                    Total=allCusotmers,
                    ThisWeek=thisWeekCustomers
                },
                Deals =
                {
                    Total= allDeals,
                    ThisWeek= thisWeekDealsCount
                },
                Revenue =
                {
                    Total=revenues,
                    ThisWeek=thisWeekrevenues
                },
                Actions =
                {
                    Total=allActions,
                    ThisWeek=thisWeekActions
                }
            };
            
        }
    }
}
