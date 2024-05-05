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
        public async Task<SalesRepresentativeReport> SalesReport(string salesId)
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

            var customers = await _unitOfWork.Customers.GetAllAsync(x => x.AdditionDate >= DateTime.UtcNow.AddDays(-30), ["SalesRepresntative"]);
            var messages = await _unitOfWork.Messages.GetAllAsync(c => c.MessageDate >= DateTime.UtcNow.AddDays(-30), ["SalesRepresntative"]);
            var calls = await _unitOfWork.Calls.GetAllAsync(x => x.CallDate >= DateTime.UtcNow.AddDays(-30), ["SalesRepresntative"]);
            var meetings = await _unitOfWork.Meetings.GetAllAsync(x => x.MeetingDate.Value >= DateTime.UtcNow.AddDays(-30), ["SalesRepresntative"]);
            var deals = await _unitOfWork.Deals.GetAllAsync(x => x.DealDate.Value >= DateTime.UtcNow.AddDays(-30), ["SalesRepresntative"]);
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
            salesreport.Revenue =(int) salesDeals.Sum(x => x.Price);
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
                DealPrice = (int)bestDeal.Price
            };
            return salesreport;
        }
    }
}
