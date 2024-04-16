using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;

namespace CRM.Core.Services.Implementations
{
    public class SharedService : ISharedService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SharedService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // will be used after adding Manager module
        public async Task<ReturnInterstsDto> GetAllInterests()
        {
            var interests = await _unitOfWork.Interests.GetAllAsync();
            if (interests == null)
            {
                return new ReturnInterstsDto
                {
                    IsSuccess = false,
                    Errors = ["No interests found"]
                };
            }
            var Interests = new List<InterestDto>();
            foreach (var interest in interests)
            {
                var interestDto = new InterestDto
                {
                    Id = interest.InterestID,
                    Name = interest.InterestName
                };
                Interests.Add(interestDto);
            }
            return new ReturnInterstsDto
            {
                IsSuccess = true,
                Interests = Interests
            };
        }
        



        // will be used after adding Manager module
        public async Task<ReturnSourcesDto> GetAllSources()
        {
            var sources = await _unitOfWork.Sources.GetAllAsync();
            if (sources == null)
            {
                return new ReturnSourcesDto
                {
                    IsSuccess = false,
                    Errors = ["No sources found"]
                };
            }
            var Sources = new List<SourceDto>();
            foreach (var source in sources)
            {
                var sourceDto = new SourceDto
                {
                    Name = source.SourceName,
                    Id = source.SourceId
                };
                Sources.Add(sourceDto);
            }
            return new ReturnSourcesDto
            {
                IsSuccess = true,
                Sources = Sources
            };
        }

        public async Task<BusinessDto> GetBussinesInfo()
        {
            var businesses = await _unitOfWork.Businesses.GetAllAsync();
            var existingBusiness = businesses.FirstOrDefault();
            if (existingBusiness == null)
            {
                return new BusinessDto();
            }
            var businessDto = new BusinessDto
            {
                CompanyName = existingBusiness.CompanyName,
                Description = existingBusiness.Description
            };
            return businessDto;
        }

        public async Task<ReturnActionDto> GetActionsForCustomer(int customerId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);

            if (customer == null)
            {
                return new ReturnActionDto
                {

                    IsSuccess = false,
                    Errors = ["Customer not found"],

                };
            }

            var calls = await _unitOfWork.Calls.GetAllAsync(call => call.Customer.CustomerId == customerId);
            var messages = await _unitOfWork.Messages.GetAllAsync(message => message.Customer.CustomerId == customerId);
            var meetings = await _unitOfWork.Meetings.GetAllAsync(meeting => meeting.Customer.CustomerId == customerId);
            var deals = await _unitOfWork.Deals.GetAllAsync(deal => deal.Customer.CustomerId == customerId, includes: new[] { "Interest" });
            var interests = await _unitOfWork.Interests.GetAllAsync();

            var actions = new List<ActionDto>();

            actions.AddRange(calls.Select(call => new ActionDto
            {
                Id = call.CallID,
                Type = "call",
                Status = (int)call.CallStatus,
                Summary = call.CallSummery,
                Date = call.CallDate,
                FollowUp = call.FollowUpDate
            }));

            actions.AddRange(messages.Select(message => new ActionDto
            {
                Id = message.MessageID,
                Type = "message",
                Summary = message.MessageContent,
                Date = message.MessageDate,
                FollowUp = message.FollowUpDate
            }));

            actions.AddRange(meetings.Select(meeting => new ActionDto
            {
                Id = meeting.MeetingID,
                Type = "meeting",
                Online = meeting.connectionState,
                Summary = meeting.MeetingSummary,
                Date = meeting.MeetingDate,
                FollowUp = meeting.FollowUpDate
            }));

            actions.AddRange(deals.Select(deal => new ActionDto
            {
                Id = deal.DealId,
                Type = "deal",
                Price = deal.Price,
                Interest = interests.FirstOrDefault(i => i.InterestID == deal.Interest.InterestID)?.ToInterestDto(),
                Summary = deal.description,
                Date = deal.DealDate
            }));

            actions = actions.OrderBy(a => a.Date).ToList();

            return new ReturnActionDto
            {

                IsSuccess = true,
                Message = "Actions Found",
                Actions = actions
            };
        }
    }
}
