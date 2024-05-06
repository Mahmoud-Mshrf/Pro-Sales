using CRM.Core.Models;
using CRM.Core.Reports;
using CRM.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        //public static bool WithinDay(DateTime date) => date.Day == DateTime.UtcNow.Day; 
        //public static bool WithinDays(DateTime date,int days) => date == DateTime.UtcNow.AddDays(-days);
        //public static bool WithinMonth(DateTime date) => date >= DateTime.UtcNow.AddDays(-30);
        //public static bool WithinWeek(DateTime date) => date >= DateTime.UtcNow.AddDays(-7);
        DateTime PastWeek = DateTime.UtcNow.AddDays(-7);
        DateTime PastMonth = DateTime.UtcNow.AddDays(-30);
        int Today = DateTime.UtcNow.Day;
        public async Task<DailyReport> MainReport(int page, int size,string within)
        {
            var salesReps = await _unitOfWork.UserManager.GetUsersInRoleAsync("Sales Representative");

            //IEnumerable<Customer> customers = new List<Customer>();
            //IEnumerable<Message> messages = new List<Message>();
            //IEnumerable<Call> calls = new List<Call>();
            //IEnumerable<Meeting> meetings = new List<Meeting>();
            //IEnumerable<Deal> deals = new List<Deal>();
            var  customers = await _unitOfWork.Customers.GetAllAsync(["SalesRepresntative"]);
            var  messages = await _unitOfWork.Messages.GetAllAsync(["SalesRepresntative"]);
            var  calls = await _unitOfWork.Calls.GetAllAsync(["SalesRepresntative"]);
            var  meetings = await _unitOfWork.Meetings.GetAllAsync(["SalesRepresntative"]);
            var  deals = await _unitOfWork.Deals.GetAllAsync(["SalesRepresntative"]);
            if (within == "Daily")
            {
                customers = customers.Where(x => x.AdditionDate.Day==Today);
                messages = messages.Where(x => x.MessageDate.Day == Today);
                calls = calls.Where(x => x.CallDate.Day == Today);
                meetings = meetings.Where(x => x.MeetingDate.Value.Day == Today);
                deals = deals.Where(x => x.DealDate.Value.Day == Today);
            }
            else if (within == "Monthly")
            {
                customers = customers.Where(x => x.AdditionDate >= PastMonth);
                messages = messages.Where(x => x.MessageDate >= PastMonth);
                calls = calls.Where(x => x.CallDate >= PastMonth);
                meetings = meetings.Where(x => x.MeetingDate.Value >= PastMonth);
                deals = deals.Where(x => x.DealDate.Value >= PastMonth);
            }
            else if(within== "Weekly")
            {
                customers = customers.Where(x => x.AdditionDate >= PastWeek);
                messages = messages.Where(x => x.MessageDate >= PastWeek);
                calls = calls.Where(x => x.CallDate >= PastWeek);
                meetings = meetings.Where(x => x.MeetingDate.Value >= PastWeek);
                deals = deals.Where(x => x.DealDate.Value >= PastWeek);
            }

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
                ActionsCount = messages.Count() + calls.Count() + deals.Count() + meetings.Count(),
                SalesReports = pages
            };
            return report;
        }
        public async Task<GlobalStat> GlobalStatAsync()
        {
            var customers =await _unitOfWork.Customers.GetAllAsync();
            var allCusotmers = customers.Count();
            // return count of this week customers
            var thisWeekCustomers = customers.Where(c => c.AdditionDate >= PastWeek).Count();
            var thisMonthCustomers = customers.Where(c => c.AdditionDate >= PastMonth).Count();
            var todayCustomers = customers.Where(c => c.AdditionDate.Day==Today).Count();

            var deals = await _unitOfWork.Deals.GetAllAsync();
            var allDeals = deals.Count();
            var thisWeekDeals = deals.Where(d => d.DealDate.Value >= PastWeek);
            var thisMonthDeals = deals.Where(d => d.DealDate.Value >= PastMonth);
            var todayDeals = deals.Where(d => d.DealDate.Value.Day == Today);
            var thisWeekDealsCount = thisWeekDeals.Count();
            var thisMonthDealsCount = thisMonthDeals.Count();
            var todayDealsCount = todayDeals.Count();

            var revenues = deals.Sum(d => d.Price);
            var thisWeekrevenues = thisWeekDeals.Sum(d => d.Price);
            var thisMonthrevenues = thisMonthDeals.Sum(d => d.Price);
            var todayrevenues = todayDeals.Sum(d => d.Price);

            var meetings = await _unitOfWork.Meetings.GetAllAsync();
            var allMeetings = meetings.Count();
            var thisWeekMeetings = meetings.Where(m=> m.MeetingDate.Value >= PastWeek).Count();
            var thisMonthMeetings = meetings.Where(m=> m.MeetingDate.Value >= PastMonth).Count();
            var todayMeetings = meetings.Where(m=> m.MeetingDate.Value.Day == Today).Count();

            var calls = await _unitOfWork.Calls.GetAllAsync();
            var allCalls = calls.Count();
            var thisWeekCalls = calls.Where(c=>c.CallDate >= PastWeek).Count();
            var thisMonthCalls = calls.Where(c=>c.CallDate >= PastMonth).Count();
            var todayCalls = calls.Where(c=>c.CallDate.Day== Today).Count();

            var messages = await _unitOfWork.Messages.GetAllAsync();
            var allMessages = messages.Count();
            var thisWeekMessages = messages.Where(m=>m.MessageDate >= PastWeek).Count();
            var thisMonthMessages = messages.Where(m=>m.MessageDate >= PastMonth).Count();
            var todayMessages = messages.Where(m => m.MessageDate.Day == Today).Count();

            var allActions = allCalls + allMessages + allMeetings + allDeals;
            var thisWeekActions = thisWeekCalls + thisWeekMessages + thisWeekMeetings + thisWeekDealsCount;
            var thisMonthActions = thisMonthCalls + thisMonthMessages + thisMonthMeetings + thisMonthDealsCount;
            var todayActions = todayCalls + todayMessages + todayMeetings + todayDealsCount;
            return new GlobalStat
            {
                Customers =
                {
                    Total=allCusotmers,
                    ThisWeek=thisWeekCustomers,
                    ThisMonth=thisMonthCustomers,
                    Today=todayCustomers
                },
                Deals =
                {
                    Total= allDeals,
                    ThisWeek= thisWeekDealsCount,
                    ThisMonth=thisMonthDealsCount,
                    Today=todayDealsCount
                },
                Revenue =
                {
                    Total=(int) revenues,
                    ThisWeek=(int) thisWeekrevenues,
                    ThisMonth=(int) thisMonthrevenues,
                    Today=(int) todayrevenues
                },
                Actions =
                {
                    Total=allActions,
                    ThisWeek=thisWeekActions,
                    ThisMonth=thisMonthActions,
                    Today=todayActions
                }
            };

        }
        public async Task<SalesRepresentativeReport> SalesReport(string salesId,string within)
        {
            var sales = await _unitOfWork.UserManager.FindByIdAsync(salesId);
            if (sales == null)
            {
                return new SalesRepresentativeReport
                {
                    IsSuccess = false,
                    Errors = ["Sales representative not found"]
                };
            }
            //IEnumerable<Customer> customers = new List<Customer>();
            //IEnumerable<Message> messages = new List<Message>();
            //IEnumerable<Call> calls = new List<Call>();
            //IEnumerable<Meeting> meetings = new List<Meeting>();
            //IEnumerable<Deal> deals = new List<Deal>();
            var customers = await _unitOfWork.Customers.GetAllAsync(["SalesRepresntative"]);
            var messages = await _unitOfWork.Messages.GetAllAsync(["SalesRepresntative"]);
            var calls = await _unitOfWork.Calls.GetAllAsync(["SalesRepresntative"]);
            var meetings = await _unitOfWork.Meetings.GetAllAsync(["SalesRepresntative"]);
            var deals = await _unitOfWork.Deals.GetAllAsync(["SalesRepresntative"]);
            if (within == "Daily")
            {
                customers = customers.Where(x => x.AdditionDate.Day == Today);
                messages = messages.Where(x => x.MessageDate.Day == Today);
                calls = calls.Where(x => x.CallDate.Day == Today);
                meetings = meetings.Where(x => x.MeetingDate.Value.Day == Today);
                deals = deals.Where(x => x.DealDate.Value.Day == Today);
            }
            else if (within == "Monthly")
            {
                customers = customers.Where(x => x.AdditionDate >= PastMonth);
                messages = messages.Where(x => x.MessageDate >= PastMonth);
                calls = calls.Where(x => x.CallDate >= PastMonth);
                meetings = meetings.Where(x => x.MeetingDate.Value >= PastMonth);
                deals = deals.Where(x => x.DealDate.Value >= PastMonth);
            }
            else if (within == "Weekly")
            {
                customers = customers.Where(x => x.AdditionDate >= PastWeek);
                messages = messages.Where(x => x.MessageDate >= PastWeek);
                calls = calls.Where(x => x.CallDate >= PastWeek);
                meetings = meetings.Where(x => x.MeetingDate.Value >= PastWeek);
                deals = deals.Where(x => x.DealDate.Value >= PastWeek);
            }
            var interests = await _unitOfWork.Interests.GetAllAsync();
            var sources = await _unitOfWork.Sources.GetAllAsync();

            var salesreport = new SalesRepresentativeReport();
            salesreport.IsSuccess = true;
            salesreport.FirstName = sales.FirstName;
            salesreport.LastName = sales.LastName;
            var salesCustomers = customers.Where(c => c.SalesRepresntative.Id == sales.Id);
            salesreport.Customers =salesCustomers.Count();
            salesreport.Messages = messages.Where(m => m.SalesRepresntative.Id == sales.Id).Count();
            salesreport.Meetings.Online = meetings.Where(m => m.SalesRepresntative.Id == sales.Id && m.connectionState.Value).Count();
            salesreport.Meetings.Offline = meetings.Where(m => m.SalesRepresntative.Id == sales.Id && !m.connectionState.Value).Count();
            var salesDeals = deals.Where(d => d.SalesRepresntative.Id == sales.Id);
            salesreport.Deals = salesDeals.Count();
            salesreport.Calls.Completed = calls.Where(d => d.SalesRepresntative.Id == sales.Id && d.CallStatus == CallStatus.Completed).Count();
            salesreport.Calls.Missed = calls.Where(d => d.SalesRepresntative.Id == sales.Id && d.CallStatus == CallStatus.Missed).Count();
            salesreport.Calls.Cancelled = calls.Where(d => d.SalesRepresntative.Id == sales.Id && d.CallStatus == CallStatus.Cancelled).Count();
            salesreport.Calls.Busy = calls.Where(d => d.SalesRepresntative.Id == sales.Id && d.CallStatus == CallStatus.Busy).Count();
            salesreport.Calls.Failed = calls.Where(d => d.SalesRepresntative.Id == sales.Id && d.CallStatus == CallStatus.Failed).Count();
            //salesreport.Revenue =(int) salesDeals.Sum(x => x.Price);
            var InterestsList = new List<ItemCount>();
            foreach (var interest in interests)
            {
                var interestitem = new ItemCount
                {
                    Name = interest.InterestName
                };
                InterestsList.Add(interestitem);
            }

            foreach (var deal in salesDeals)
            {
                foreach(var item in InterestsList)
                {
                    if(deal.Interest.InterestName==item.Name)
                    {
                        item.Count++;
                        item.Revenue += deal.Price;
                        break;
                    }
                }
            }
            salesreport.DoneDeals = InterestsList;

            var sourcesList = new List<ItemCount>();
            foreach (var source in sources)
            {
                var sourceitem = new ItemCount
                {
                    Name = source.SourceName
                };
                sourcesList.Add(sourceitem);
            }
            ///////////
            foreach (var customer in salesCustomers)
            {
                foreach (var item in sourcesList)
                {
                    if (customer.Source.SourceName == item.Name)
                    {
                        item.Count++;
                        break;
                    }
                }
            }
            salesreport.Sources = sourcesList;
            var orderdDeals = salesDeals.OrderByDescending(x=>x.Price).ToList();
            var bestDeal = orderdDeals.FirstOrDefault();
            if (bestDeal != null)
            {
                salesreport.BestDeal = new BestDeal
                {
                    CustomerFirstName = bestDeal.Customer.FirstName,
                    CustomerLastName = bestDeal.Customer.LastName,
                    InterestName = bestDeal.Interest.InterestName,
                    DealPrice = bestDeal.Price
                };
            }
            
            return salesreport;
        }
    }
}
