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
        static bool WithinDay(DateTime date) => date.Day == DateTime.UtcNow.Day; 
        static bool WithinDays(DateTime date,int days) => date == DateTime.UtcNow.AddDays(-days);
        static bool WithinMonth(DateTime date) => date == DateTime.UtcNow.AddDays(-30);
        static bool WithinWeek(DateTime date) => date == DateTime.UtcNow.AddDays(-7);
        public async Task<DailyReport> MainReport(int page, int size,string within)
        {
            var salesReps = await _unitOfWork.UserManager.GetUsersInRoleAsync("Sales Representative");

            IEnumerable<Customer> customers = new List<Customer>();
            IEnumerable<Message> messages = new List<Message>();
            IEnumerable<Call> calls = new List<Call>();
            IEnumerable<Meeting> meetings = new List<Meeting>();
            IEnumerable<Deal> deals = new List<Deal>();

            if (within == "Daily")
            {
                customers = await _unitOfWork.Customers.GetAllAsync(x => WithinDay(x.AdditionDate), ["SalesRepresntative"]);
                messages = await _unitOfWork.Messages.GetAllAsync(x => WithinDay(x.MessageDate), ["SalesRepresntative"]);
                calls = await _unitOfWork.Calls.GetAllAsync(x => WithinDay(x.CallDate), ["SalesRepresntative"]);
                meetings = await _unitOfWork.Meetings.GetAllAsync(x => WithinDay(x.MeetingDate.Value), ["SalesRepresntative"]);
                deals = await _unitOfWork.Deals.GetAllAsync(x => WithinDay(x.DealDate.Value), ["SalesRepresntative"]);
            }
            else if (within == "Monthly")
            {
                customers = await _unitOfWork.Customers.GetAllAsync(x => WithinMonth(x.AdditionDate), ["SalesRepresntative"]);
                messages = await _unitOfWork.Messages.GetAllAsync(x => WithinMonth(x.MessageDate), ["SalesRepresntative"]);
                calls = await _unitOfWork.Calls.GetAllAsync(x => WithinMonth(x.CallDate), ["SalesRepresntative"]);
                meetings = await _unitOfWork.Meetings.GetAllAsync(x => WithinMonth(x.MeetingDate.Value), ["SalesRepresntative"]);
                deals = await _unitOfWork.Deals.GetAllAsync(x => WithinMonth(x.DealDate.Value), ["SalesRepresntative"]);
            }
            else if(within== "Weekly")
            {
                customers = await _unitOfWork.Customers.GetAllAsync(x => WithinWeek(x.AdditionDate), ["SalesRepresntative"]);
                messages = await _unitOfWork.Messages.GetAllAsync(x => WithinWeek(x.MessageDate), ["SalesRepresntative"]);
                calls = await _unitOfWork.Calls.GetAllAsync(x => WithinWeek(x.CallDate), ["SalesRepresntative"]);
                meetings = await _unitOfWork.Meetings.GetAllAsync(x => WithinWeek(x.MeetingDate.Value), ["SalesRepresntative"]);
                deals = await _unitOfWork.Deals.GetAllAsync(x => WithinWeek(x.DealDate.Value), ["SalesRepresntative"]);
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
            var thisWeekCustomers = customers.Where(c => WithinWeek(c.AdditionDate)).Count();
            
            var deals = await _unitOfWork.Deals.GetAllAsync();
            var allDeals = deals.Count();
            var thisWeekDeals = deals.Where(d => WithinWeek(d.DealDate.Value));
            var thisWeekDealsCount = thisWeekDeals.Count();

            var revenues = deals.Sum(d => d.Price);
            var thisWeekrevenues = thisWeekDeals.Sum(d => d.Price);

            var meetings = await _unitOfWork.Meetings.GetAllAsync();
            var allMeetings = meetings.Count();
            var thisWeekMeetings = meetings.Where(m=>WithinWeek(m.MeetingDate.Value)).Count();

            var calls = await _unitOfWork.Calls.GetAllAsync();
            var allCalls = calls.Count();
            var thisWeekCalls = calls.Where(c=>WithinWeek(c.CallDate)).Count();

            var messages = await _unitOfWork.Messages.GetAllAsync();
            var allMessages = messages.Count();
            var thisWeekMessages = messages.Where(m=>WithinWeek(m.MessageDate)).Count();

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
                    Total=(int) revenues,
                    ThisWeek=(int) thisWeekrevenues
                },
                Actions =
                {
                    Total=allActions,
                    ThisWeek=thisWeekActions
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

            IEnumerable<Customer> customers = new List<Customer>();
            IEnumerable<Message> messages = new List<Message>();
            IEnumerable<Call> calls = new List<Call>();
            IEnumerable<Meeting> meetings = new List<Meeting>();
            IEnumerable<Deal> deals = new List<Deal>();
            if (within == "Daily")
            {
                customers = await _unitOfWork.Customers.GetAllAsync(x => WithinDay(x.AdditionDate), ["SalesRepresntative"]);
                messages = await _unitOfWork.Messages.GetAllAsync(x => WithinDay(x.MessageDate), ["SalesRepresntative"]);
                calls = await _unitOfWork.Calls.GetAllAsync(x => WithinDay(x.CallDate), ["SalesRepresntative"]);
                meetings = await _unitOfWork.Meetings.GetAllAsync(x => WithinDay(x.MeetingDate.Value), ["SalesRepresntative"]);
                deals = await _unitOfWork.Deals.GetAllAsync(x => WithinDay(x.DealDate.Value), ["SalesRepresntative"]);
            }
            else if (within == "Monthly")
            {
                customers = await _unitOfWork.Customers.GetAllAsync(x => WithinMonth(x.AdditionDate), ["SalesRepresntative"]);
                messages = await _unitOfWork.Messages.GetAllAsync(x => WithinMonth(x.MessageDate), ["SalesRepresntative"]);
                calls = await _unitOfWork.Calls.GetAllAsync(x => WithinMonth(x.CallDate), ["SalesRepresntative"]);
                meetings = await _unitOfWork.Meetings.GetAllAsync(x => WithinMonth(x.MeetingDate.Value), ["SalesRepresntative"]);
                deals = await _unitOfWork.Deals.GetAllAsync(x => WithinMonth(x.DealDate.Value), ["SalesRepresntative"]);
            }
            else if (within == "Weekly")
            {
                customers = await _unitOfWork.Customers.GetAllAsync(x => WithinWeek(x.AdditionDate), ["SalesRepresntative"]);
                messages = await _unitOfWork.Messages.GetAllAsync(x => WithinWeek(x.MessageDate), ["SalesRepresntative"]);
                calls = await _unitOfWork.Calls.GetAllAsync(x => WithinWeek(x.CallDate), ["SalesRepresntative"]);
                meetings = await _unitOfWork.Meetings.GetAllAsync(x => WithinWeek(x.MeetingDate.Value), ["SalesRepresntative"]);
                deals = await _unitOfWork.Deals.GetAllAsync(x => WithinWeek(x.DealDate.Value), ["SalesRepresntative"]);
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
            salesreport.BestDeal = new BestDeal
            {
                CustomerFirstName = bestDeal.Customer.FirstName,
                CustomerLastName = bestDeal.Customer.LastName,
                InterestName = bestDeal.Interest.InterestName,
                DealPrice = bestDeal.Price
            };
            return salesreport;
        }
    }
}
