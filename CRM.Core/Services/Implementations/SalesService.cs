using CRM.Core.Dtos;
using CRM.Core.Models;
using CRM.Core.Services.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CRM.Core.Services.Implementations
{
    public class SalesService : ISalesRepresntative
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region ManageCalls
        public async Task<ResultDto> AddCall(CallDto callDto, string salesRepEmail)
        {
            try
            {
                if (callDto == null)
                {
                    throw new ArgumentNullException(nameof(callDto));
                }

                var call = new Call();

                var customer = await _unitOfWork.Customers.GetByIdAsync(callDto.CustomerId);
                if (customer == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Customer not found"]
                    };
                }

                if (string.IsNullOrEmpty(salesRepEmail))
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Sales Representative email is null or empty"]
                    };
                }

                var salesRep = await _unitOfWork.UserManager.FindByEmailAsync(salesRepEmail);
                if (salesRep == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Sales Representative not found"]
                    };
                }

                call.CallDate = callDto.CallDate;
                call.CallStatus = callDto.CallStatus;
                call.CallSummery = callDto.CallSummery;
                call.SalesRepresntative = salesRep;
                call.Customer= customer;

                await _unitOfWork.Calls.AddAsync(call);

                try
                {
                    _unitOfWork.complete();
                }
                catch (Exception e)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Message = e.Message
                    };
                }

                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Call added successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [ex.Message]
                };
            }
        }

        public async Task<ResultDto> UpdateCallInfo(CallDto callDto, int callId)
        {
            var call = await _unitOfWork.Calls.FindAsync(c => c.CallID == callId);
            if (call == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors=["Call not Found"]
                };
            }
            var customer = await _unitOfWork.Customers.GetByIdAsync(callDto.CustomerId);
            if (customer == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Customer not Found"]
                };
            }
            call.CallSummery= callDto.CallSummery;
            call.FollowUpDate = callDto.FollowUpDate;
            try
            {
                _unitOfWork.complete();
            }
            catch (Exception e)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                   Errors = [e.Message]
                };
            }
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Call updated successfully"
            };
        }


        public async Task<ReturnCallsDto> GetAllCalls()
        {
            try
            {
                var includes = new string[] { "Customer" }; 
                var calls = await _unitOfWork.Calls.GetAllAsync(call => true, int.MaxValue, 0, includes);

                if (calls == null || !calls.Any())
                {
                    return new ReturnCallsDto
                    {
                        IsSuccess = false,
                        Errors = ["Calls not Found"]
                        
                    };
                }

                var Calls = new List<CallDto>();
                foreach (var call in calls)
                {
                    var callDto = new CallDto
                    {
                        CallDate = call.CallDate,
                        CallStatus = call.CallStatus,
                        CallSummery = call.CallSummery,
                        // Check if Customer property is not null before accessing CustomerId
                        CustomerId = call.Customer != null ? call.Customer.CustomerId : 0, // Or default value as per your requirement
                        FollowUpDate = call.FollowUpDate,
                    };
                    Calls.Add(callDto);
                }


                return new ReturnCallsDto
                {
                    IsSuccess = true,
                    Message = "Calls found",
                    Calls = Calls
                };
            }
            catch (Exception ex)
            {
                return new ReturnCallsDto
                {
                    IsSuccess = false,
                    Errors=[ex.Message]
                };
            }
        }
        public async Task<ReturnCallsDto> GetCallById(int callId)
        {
            try
            {
                var includes = new string[] { "Customer" }; 
                var call = await _unitOfWork.Calls.FindAsync(c => c.CallID == callId, includes);

                if (call == null)
                {
                    return new ReturnCallsDto
                    {
                        IsSuccess = false,
                      Errors = ["Call not Found"]
                    };
                }

                var callDto = new CallDto
                {
                    CallDate = call.CallDate,
                    CallStatus = call.CallStatus,
                    CallSummery = call.CallSummery,
                    // Check if Customer property is not null before accessing CustomerId
                    CustomerId = call.Customer != null ? call.Customer.CustomerId : 0, // Or default value as per your requirement
                    FollowUpDate = call.FollowUpDate,
                };

                return new ReturnCallsDto
                {
                    IsSuccess = true,
                    Message = "Call found",
                    Calls = new List<CallDto> { callDto }
                };
            }
            catch (Exception ex)
            {
                return new ReturnCallsDto
                {
                    IsSuccess = false,
                   Errors = [ex.Message]    
                };
            }
        }
         public async Task<ResultDto> DeleteCallById(int callId)
        {
            try
            {
                var call = await _unitOfWork.Calls.GetByIdAsync(callId);
                if (call == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Call not Found"]
                    };
                }

                _unitOfWork.Calls.Delete(call);
                try
                {
                    _unitOfWork.complete();
                }
                catch (Exception e)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                       Errors = [e.Message] 
                    };
                }

                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Call deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [ ex.Message]
                };
            }
        }

        #endregion
        #region ManageMessages
        public async Task<ResultDto> AddMessage(MessageDto messageDto, string salesRepEmail)
        {
            try
            {
                if ( messageDto== null)
                {
                    throw new ArgumentNullException(nameof(messageDto));
                }

                var message = new Message();

                var customer = await _unitOfWork.Customers.GetByIdAsync(messageDto.CustomerId);
                if (customer == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                       Errors = ["Customer not Found"]
                    };
                }

                if (string.IsNullOrEmpty(salesRepEmail))
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Sales Representative email is null or empty"]

                    };
                }

                var salesRep = await _unitOfWork.UserManager.FindByEmailAsync(salesRepEmail);
                if (salesRep == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Sales Representative not found"]
                    };
                }

                message.MessageDate = messageDto.MessageDate;
                message.MessageContent = messageDto.MessageContent;
                message.SalesRepresntative = salesRep;
                message.Customer = customer;

                await _unitOfWork.Messages.AddAsync(message);

                try
                {
                    _unitOfWork.complete();
                }
                catch (Exception e)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = [e.Message]
                    };
                }

                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Message is added successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [ex.Message]   
                };
            }

        }

        public async Task<ResultDto> UpdateMessageInfo(MessageDto messageDto, int MessageId)
        {
            var message = await _unitOfWork.Messages.FindAsync(c => c.MessageID == MessageId);
            if (message == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                   Errors = ["Message not Found"]
                };
            }
            var customer = await _unitOfWork.Customers.GetByIdAsync(messageDto.CustomerId);
            if (customer == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Customer not Found"]

                };
            }
            message.MessageContent = messageDto.MessageContent;
            message.FollowUpDate= messageDto.FollowUpDate;
            message.Customer.CustomerId = messageDto.CustomerId;
            try
            {
                _unitOfWork.complete();
            }
            catch (Exception e)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [e.Message]
                };
            }
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Message updated successfully"
            };
        }

   
        public async Task<ReturnMessagesDto> GetAllMessages()
        {
            try
            {
                var includes = new string[] { "Customer" };
                var messages = await _unitOfWork.Messages.GetAllAsync(message => true, int.MaxValue, 0, includes);

                if (messages == null || !messages.Any())
                {
                    return new ReturnMessagesDto
                    {
                        IsSuccess = false,
                        Errors = ["Message not Found"]
                    };
                }

                var Messages = new List<MessageDto>();
                foreach (var message in messages)
                {
                    var messageDto = new MessageDto
                    {
                        MessageDate = message.MessageDate,
                        MessageContent = message.MessageContent,
                        // Check if Customer property is not null before accessing CustomerId
                        CustomerId = message.Customer != null ? message.Customer.CustomerId : 0, // Or default value as per your requirement
                        FollowUpDate = message.FollowUpDate,
                    };
                    Messages.Add(messageDto);
                }


                return new ReturnMessagesDto
                {
                    IsSuccess = true,
                    Message = "Message found",
                    Messages = Messages
                };
            }
            catch (Exception ex)
            {
                return new ReturnMessagesDto
                {
                    IsSuccess = false,
                   Errors = [ex.Message]
                };
            }
        }

        public async Task<ReturnMessagesDto> GetMessageById(int messageID)
        {
            try
            {
                var includes = new string[] { "Customer" }; 
                var message = await _unitOfWork.Messages.FindAsync(c => c.MessageID == messageID, includes);

                if (message == null)
                {
                    return new ReturnMessagesDto
                    {
                        IsSuccess = false,
                       Errors = ["Message not Found"]
                    };
                }

                var messageDto = new MessageDto
                {
                    MessageDate= message.MessageDate,
                    MessageContent=message.MessageContent,
                   // Check if Customer property is not null before accessing CustomerId
                    CustomerId = message.Customer != null ? message.Customer.CustomerId : 0, // Or default value as per your requirement
                    FollowUpDate = message.FollowUpDate,
                };

                return new ReturnMessagesDto
                {
                    IsSuccess = true,
                    Message = "Message found",
                    Messages = new List<MessageDto> { messageDto}
                };
            }
            catch (Exception ex)
            {
                return new ReturnMessagesDto
                {
                    IsSuccess = false,
                    Errors = [ex.Message]   
                };
            }
        }

        public async Task<ResultDto> DeleteMessageById(int MessageId)
        {
            try
            {
                var message = await _unitOfWork.Messages.GetByIdAsync(MessageId);
                if (message == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Message not Found"]
                    };
                }

                _unitOfWork.Messages.Delete(message);
                try
                {
                    _unitOfWork.complete();
                }
                catch (Exception e)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                       Errors = [e.Message]
                    };
                }

                return new ResultDto
                {
                    IsSuccess = true,
                    Message = " Message deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [ex.Message]   
                };
            }
        }

        #endregion

        #region ManageMeetings
        public async Task<ResultDto> AddMeeting(MeetingDto meetingDto, string SaleRepEmail)
        {
            try
            {
                if (meetingDto == null)
                {
                    throw new ArgumentNullException(nameof(MeetingDto));
                }
                var meeting = new Meeting();
                var customer = await _unitOfWork.Customers.GetByIdAsync(meetingDto.CustomerId);
                if (customer == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors=["Customer not Found"]
                    };
                }

                if (string.IsNullOrEmpty(SaleRepEmail))
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Sales Representative email is null or empty"]

                    };
                }

                var salesRep = await _unitOfWork.UserManager.FindByEmailAsync(SaleRepEmail);

                if (salesRep == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Sales Representative not found"]
                    };
                }

                meeting.connectionState = meetingDto.connectionState;
                meeting.SalesRepresntative = salesRep;
                meeting.Customer = customer;
                meeting.FollowUpDate = meetingDto.FollowUpDate;
                meeting.MeetingDate = meetingDto.MeetingDate;
                meeting.MeetingSummary = meetingDto.MeetingSummary;

                await _unitOfWork.Meetings.AddAsync(meeting);

                try
                {
                    _unitOfWork.complete();
                }

                catch (Exception e)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = [e.Message]
                    };

                }

                return new ResultDto
                {
                    IsSuccess=true,
                    Message = "Meeting is added Successfully"

                };

            }

            catch (Exception e)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [e.Message]
                };
            }
        }

        public async Task<ResultDto> UpdateMeeting(MeetingDto meetingDto, int MeetingId)
        {
            var meeting = await _unitOfWork.Meetings.FindAsync(c => c.MeetingID == MeetingId);
            if (meeting == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors=["Meeting not Found"]
                };
            }
            var customer = await _unitOfWork.Customers.GetByIdAsync(meetingDto.CustomerId);
            if (customer == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Customer not Found"]
                };
            }
            meeting.connectionState = meetingDto.connectionState;
            meeting.Customer=customer;
            meeting.FollowUpDate = meetingDto.FollowUpDate;
            meeting.MeetingSummary = meetingDto.MeetingSummary;
            try
            {
                _unitOfWork.complete();
            }
            catch (Exception e)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [e.Message]
                };
            }
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Meeting updated successfully"
            };
        }

        public async Task<ReturnMeetingsDto> GetAllMeetings()
        {
            try
            {
                var includes = new string[] { "Customer" };
                var meetings = await _unitOfWork.Meetings.GetAllAsync(meetings => true, int.MaxValue, 0, includes);

                if (meetings == null || !meetings.Any())
                {
                    return new ReturnMeetingsDto
                    {
                        IsSuccess = false,
                        Errors = ["Meetings not Found"]
                        
                    };
                }

                var Meetings = new List<MeetingDto>();
                foreach (var meeting in meetings)
                {
                    var meetingDto = new MeetingDto
                    {
                        MeetingDate = meeting.MeetingDate,
                        MeetingSummary = meeting.MeetingSummary,
                        // Check if Customer property is not null before accessing CustomerId
                        CustomerId = meeting.Customer != null ? meeting.Customer.CustomerId : 0, // Or default value as per your requirement
                        FollowUpDate = meeting.FollowUpDate,
                    };
                    Meetings.Add(meetingDto);
                }


                return new ReturnMeetingsDto
                {
                    IsSuccess = true,
                    Message = "Meeting found",
                    Meetings = Meetings
                };
            }
            catch (Exception ex)
            {
                return new ReturnMeetingsDto
                {
                    IsSuccess = false,
                    Errors=[ex.Message]
                };
            }

        }

        public async Task<ReturnMeetingsDto> GetMeetingByID(int MeetingId)
        {
            try
            {
                var includes = new string[] { "Customer" };
                var meeting = await _unitOfWork.Meetings.FindAsync(c => c.MeetingID == MeetingId, includes);

                if (meeting == null)
                {
                    return new ReturnMeetingsDto
                    {
                        IsSuccess = false,
                        Errors = ["Meeting not Found"]
                    };
                }

                var meetingDto = new MeetingDto
                {
                    MeetingDate = meeting.MeetingDate,
                    MeetingSummary = meeting.MeetingSummary,
                    // Check if Customer property is not null before accessing CustomerId
                    CustomerId = meeting.Customer != null ? meeting.Customer.CustomerId : 0, // Or default value as per your requirement
                    FollowUpDate = meeting.FollowUpDate,
                };

                return new ReturnMeetingsDto
                {
                    IsSuccess = true,
                    Message = "Meeting found",
                    Meetings = new List<MeetingDto> { meetingDto }
                };
            }
            catch (Exception ex)
            {
                return new ReturnMeetingsDto
                {
                    IsSuccess = false,
                   Errors = [ex.Message]
                };
            }
        }

        public async Task<ResultDto> DeleteMeeting(int MeetingId)
        {
            try
            {
                var meeting = await _unitOfWork.Meetings.GetByIdAsync(MeetingId);
                if (meeting == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Meeting not Found"]
                    };
                }

                _unitOfWork.Meetings.Delete(meeting);
                try
                {
                    _unitOfWork.complete();
                }
                catch (Exception e)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = [e.Message]
                    };
                }

                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Meeting is deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [ex.Message]
                };
            }
        }
        #endregion

        #region ManageDeals
        public async Task<ResultDto> AddDeal(DealsDto dealsDto, string salesRepEmail)
        {
            try
            {
                if (dealsDto == null)
                {
                    throw new ArgumentNullException(nameof(dealsDto));
                }

                var deal = new Deal();

                var customer = await _unitOfWork.Customers.GetByIdAsync(dealsDto.CustomerId);
                if (customer == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                       Errors = ["Customer not Found"]
                    };
                }
                var interest = await _unitOfWork.Interests.GetByIdAsync(dealsDto.InterestId);
                if (interest == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors=[ "Interest not found"]
                    };
                }

                if (string.IsNullOrEmpty(salesRepEmail))
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Sales Representative email is null or empty"]
                    };
                }

                var salesRep = await _unitOfWork.UserManager.FindByEmailAsync(salesRepEmail);
                if (salesRep == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors = ["Sales Representative not found"]
                    };
                }

                deal.DealDate = dealsDto.DealDate;
                deal.description = dealsDto.description;
                deal.Price = dealsDto.Price;
                deal.Interest = interest;
                deal.SalesRepresntative = salesRep;
                deal.Customer = customer;

                await _unitOfWork.Deals.AddAsync(deal);

                try
                {
                    _unitOfWork.complete();
                }
                catch (Exception e)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                       Errors = [e.Message]
                    };
                }

                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Deal added successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors =[ ex.Message]
                };
            }
        }


        public async Task<ResultDto> UpdateDeal(DealsDto dealsDto, int dealId)
        {
            var deal = await _unitOfWork.Deals.FindAsync(c => c.DealId == dealId);
            if (deal == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors =[ "Deal not found"]
                };
            }
            var customer = await _unitOfWork.Customers.GetByIdAsync(dealsDto.CustomerId);
            if (customer == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors =[ "Customer not found"]
                };
            }
            var interest = await _unitOfWork.Interests.GetByIdAsync(dealsDto.InterestId);
            if (interest == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors =[ "Interest not found"]
                };
            }

            deal.Price = dealsDto.Price;
            deal.DealDate = dealsDto.DealDate;
            deal.description = dealsDto.description;
            deal.Interest=interest;
            deal.Customer=customer;
            
            try
            {
                _unitOfWork.complete();
            }
            catch (Exception e)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors =[e.Message]
                };
            }
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Deal updated successfully"
            };
        }

        public async Task<ReturnDealsDto> GetAllDeals()
        {
            try
            {
                var includes = new string[] { "Customer","Interest" };
                var deals = await _unitOfWork.Deals.GetAllAsync(call => true, int.MaxValue, 0, includes);

                if (deals == null || !deals.Any())
                {
                    return new ReturnDealsDto
                    {
                        IsSuccess = false,
                        Errors = ["Deals not Found"]
                    };
                }
                 
                var Deals = new List<DealsDto>();
                foreach (var deal in deals)
                {
                    var dealDto = new DealsDto
                    {
                        DealDate= deal.DealDate,
                        description=deal.description,
                        Price= deal.Price,    
                        // Check if Customer property is not null before accessing CustomerId
                        CustomerId = deal.Customer != null ? deal.Customer.CustomerId : 0,
                        InterestId = deal.Interest != null? deal.Interest.InterestID:0,
                       
                    };
                    Deals.Add(dealDto);
                }


                return new ReturnDealsDto
                {
                    IsSuccess = true,
                    Message = "Calls found",
                    Deals = Deals
                };
            }
            catch (Exception ex)
            {
                return new ReturnDealsDto
                {
                    IsSuccess = false,
                    Errors =[ ex.Message]
                };
            }
        }

        public async Task<ReturnDealsDto> GetDealById(int DealId)
        {
            try
            {
                var includes = new string[] { "Customer","Interest" };
                var deal = await _unitOfWork.Deals.FindAsync(c => c.DealId == DealId, includes);

                if (deal == null)
                {
                    return new ReturnDealsDto
                    {
                        IsSuccess = false,
                       Errors =[ "Deal not found"]
                    };
                }

                var dealDto = new DealsDto
                {
                   description=deal.description,
                   Price= deal.Price,
                   DealDate=deal.DealDate,
                   InterestId=deal.Interest !=null? deal.Interest.InterestID:0,
                    // Check if Customer property is not null before accessing CustomerId
                    CustomerId = deal.Customer != null ? deal.Customer.CustomerId : 0, // Or default value as per your requirement
                    
                };

                return new ReturnDealsDto
                {
                    IsSuccess = true,
                    Message = "Deal found",
                    Deals = new List<DealsDto> { dealDto }
                };
            }
            catch (Exception ex)
            {
                return new ReturnDealsDto
                {
                    IsSuccess = false,
                    Errors =[ ex.Message]
                };
            }
        }

        public async Task<ResultDto> DeleteDeal(int dealId)
        {
            try
            {
                var deal = await _unitOfWork.Deals.GetByIdAsync(dealId);
                if (deal == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors =[ "Deal not found"]
                    };
                }

                _unitOfWork.Deals.Delete(deal);
                try
                {
                    _unitOfWork.complete();
                }
                catch (Exception e)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Errors =[e.Message]
                    };
                }

                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Deal deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors =[ex.Message]
                };
            }
        }
        #endregion




    }
}
