using CRM.Core.Dtos;
using CRM.Core.Models;
using CRM.Core.Services.Interfaces;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;

namespace CRM.Core.Services.Implementations
{
    public class SalesService : ISalesRepresntative
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISharedService _sharedService;
        private readonly IFilterService _filterService;
        private readonly UserManager<ApplicationUser> _usermanager;

        public SalesService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ISharedService sharedService, IFilterService filterService, UserManager<ApplicationUser> usermanager)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _sharedService = sharedService;
            _filterService = filterService;
            _usermanager = usermanager;
        }
          
        #region ManageCalls

        public async Task<ActionResult<IEnumerable<object>>> AddCall(AddCallDto addcallDto, string salesRepEmail)
        {
            if (addcallDto == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "AddCallDto cannot be null" } });
            }

            if (string.IsNullOrEmpty(salesRepEmail))
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative email is null or empty" } });
            }

            var calls = new List<object>();

            if (!addcallDto.CustomerId.HasValue)  
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "CustomerId is required" } });
            }

            var customerId = addcallDto.CustomerId.Value;  
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Customer not found" } });
            }
            if (addcallDto.status == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "The status field is required." } });
            }
            if (addcallDto.date == default(DateTime))
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "The date field is required." } });
            }

            var salesRep = await _unitOfWork.UserManager.FindByEmailAsync(salesRepEmail);
            if (salesRep == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative not found" } });
            }

            var call = new Call
            {
                CallStatus = addcallDto.status,
                CallSummery = addcallDto.summary,
                CallDate = addcallDto.date,
              //  FollowUpDate = addcallDto.followUp,
                SalesRepresntative = salesRep,
                Customer = customer  
            };
            if (addcallDto.followUp == null)
            {
                call.FollowUpDate = null;
            }
            else
            {
                call.FollowUpDate = addcallDto.followUp;
            }

            await _unitOfWork.Calls.AddAsync(call);
            try
            {
                _unitOfWork.complete(); 
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { e.Message } });
            }

            calls.Add(new CallDto
            {
                id = call.CallID,
                status = call.CallStatus,
                summary = call.CallSummery,
                date = call.CallDate,
                followUp = call.FollowUpDate,
                CustomerId = customer.CustomerId
            });

            return new OkObjectResult(calls);  
        }





        //public async Task<ActionResult<IEnumerable<object>>> AddCall(AddCallDto addcallDto, string salesRepEmail)
        //{
        //    try
        //    {
        //        if (addcallDto == null)
        //        {
        //            throw new ArgumentNullException(nameof(addcallDto));
        //        }

        //        var calls = new List<object>();



        //        var customer = await _unitOfWork.Customers.GetByIdAsync(addcallDto.CustomerId);
        //        if (customer == null)
        //        {
        //            return new BadRequestObjectResult(new { errors = new List<string> { "Customer not found" } });
        //        }

        //        if (string.IsNullOrEmpty(salesRepEmail))
        //        {
        //            return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative email is null or empty" } });
        //        }

        //        var salesRep = await _unitOfWork.UserManager.FindByEmailAsync(salesRepEmail);
        //        if (salesRep == null)
        //        {
        //            return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative not found" } });
        //        }

        //        var call = new Call
        //        {
        //            CallStatus = addcallDto.status,
        //            CallSummery = addcallDto.summary,
        //            CallDate = addcallDto.date,
        //            FollowUpDate = addcallDto.followUp,
        //            SalesRepresntative = salesRep,
        //           Customer = customer
        //        };

        //        await _unitOfWork.Calls.AddAsync(call);
        //        try
        //        {
        //            _unitOfWork.complete();
        //        }
        //        catch (Exception e)
        //        {
        //            return new BadRequestObjectResult(new { errors = new List<string> { e.Message } });
        //        }

        //        calls.Add(new CallDto
        //        {
        //            id = call.CallID,
        //            status = call.CallStatus,
        //            summary = call.CallSummery,
        //            date = call.CallDate,
        //            followUp = call.FollowUpDate,
        //            CustomerId = call.Customer.CustomerId
        //        });

        //        return new OkObjectResult(calls);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BadRequestObjectResult(new { errors = new List<string> { ex.Message } });
        //    }
        //}

        public async Task<ActionResult<IEnumerable<CallDto>>> UpdateCallInfo(UpdateCallDto addCallDto, string callId)
        {
            
            var includes = new string[] { "Customer" };
            var call = await _unitOfWork.Calls.FindAsync(c => c.CallID == callId, includes);

           
            if (call == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Call not found" } });
            }

           
            if (addCallDto.CustomerId.HasValue)  
            {
                var newCustomer = await _unitOfWork.Customers.GetByIdAsync(addCallDto.CustomerId.Value);

                if (newCustomer == null) 
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { " customer not found" } });
                }

                call.Customer = newCustomer;  
            }

           
            if (addCallDto.status != null)
            {
                call.CallStatus = addCallDto.status;
            }

            if (!string.IsNullOrEmpty(addCallDto.summary))
            {
                call.CallSummery = addCallDto.summary;
            }

            if (addCallDto.date != default(DateTime))
            {
                call.CallDate = addCallDto.date;
            }

            if (addCallDto.followUp != default(DateTime))
            {
                call.FollowUpDate = addCallDto.followUp;
            }

            
            try
            {
                _unitOfWork.complete();  
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { ex.Message } });
            }

            
            var callDto = new CallDto
            {
                id = call.CallID,
                status = call.CallStatus,
                summary = call.CallSummery,
                date = call.CallDate,
                followUp = call.FollowUpDate,
                CustomerId = call.Customer != null ? call.Customer.CustomerId : 0,
            };

           
            return new OkObjectResult(new List<CallDto> { callDto });
        }



        //public async Task<ActionResult<IEnumerable<CallDto>>> UpdateCallInfo(AddCallDto addCallDto, string callId)
        //{
        //    var includes = new string[] { "Customer" };
        //    var call = await _unitOfWork.Calls.FindAsync(c => c.CallID == callId, includes);

        //    // Return an error if the call is not found
        //    if (call == null)
        //    {
        //        return new BadRequestObjectResult(new { errors = new List<string> { "Call not found" } });
        //    }

        //    // Update call fields with incoming request data without considering CustomerId
        //    if (addCallDto.status != null)
        //    {
        //        call.CallStatus = addCallDto.status;
        //    }

        //    if (addCallDto.summary != null)
        //    {
        //        call.CallSummery = addCallDto.summary;
        //    }

        //    if (addCallDto.date != default(DateTime))
        //    {
        //        call.CallDate = addCallDto.date;
        //    }

        //    if (addCallDto.followUp != default(DateTime))
        //    {
        //        call.FollowUpDate = addCallDto.followUp;
        //    }

        //    // Commit the changes and handle potential exceptions
        //    try
        //    {
        //        _unitOfWork.complete();
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BadRequestObjectResult(new { errors = new List<string> { ex.Message } });
        //    }

        //    // Create the response CallDto
        //    var callDto = new CallDto
        //    {
        //        id = call.CallID,
        //        type = ActionType.call,
        //        status = call.CallStatus,
        //        summary = call.CallSummery,
        //        date = call.CallDate,
        //        followUp = call.FollowUpDate,
        //        CustomerId = call.Customer != null ? call.Customer.CustomerId : 0,
        //    };

        //    // Return the updated call information
        //    return new OkObjectResult(new List<CallDto> { callDto });
        //}








        public async Task<IEnumerable<CallDto>> GetAllCalls()
        {
            try
            {
                var includes = new string[] { "Customer" };
                var calls = await _unitOfWork.Calls.GetAllAsync(call => true, int.MaxValue, 0, includes);

                if (calls == null || !calls.Any())
                {
                    return null; 
                }

                var Calls = new List<CallDto>();
                foreach (var call in calls)
                {
                    var callDto = new CallDto
                    {
                        id = call.CallID,
                        type = ActionType.call,
                        status = call.CallStatus,
                        summary = call.CallSummery,
                        date = call.CallDate,
                        followUp = call.FollowUpDate,
                        CustomerId = call.Customer != null ? call.Customer.CustomerId : 0,


                    };
                    Calls.Add(callDto);
                }

                return Calls;
            }
            catch 
            {
                
                return Enumerable.Empty<CallDto>(); 
            }
        }

        public async Task<IEnumerable<CallDto>> GetCallById(string callId)
        {
            try
            {
                var includes = new string[] { "Customer" };
                var call = await _unitOfWork.Calls.FindAsync(c => c.CallID == callId, includes);

                if (call == null)
                {
                    return null ; 
                }

                var callDto = new CallDto
                {
                    id = call.CallID,
                    type = ActionType.call,
                    status = call.CallStatus,
                    summary = call.CallSummery,
                    date = call.CallDate,
                    followUp = call.FollowUpDate,
                    CustomerId = call.Customer != null ? call.Customer.CustomerId : 0,
                };

                return new List<CallDto> { callDto };
            }
            catch 
            {
                
                return Enumerable.Empty<CallDto>(); 
            }
        }

        public async Task<ResultDto> DeleteCallById(string callId)
        {
            try
            {
                var call = await _unitOfWork.Calls.FindAsync(c => c.CallID == callId);
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
                    Message =  "Call deleted successfully" 
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
        public async Task<ActionResult<IEnumerable<object>>> AddMessage(AddMessageDto messageDto, string salesRepEmail)
        {
            try
            {

                if (messageDto == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "AddCallDto cannot be null" } });
                }

                if (string.IsNullOrEmpty(salesRepEmail))
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative email is null or empty" } });
                }

                var messages = new List<object>();

                if (!messageDto.CustomerId.HasValue)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "CustomerId is required" } });
                }

                var customerId = messageDto.CustomerId.Value;
                var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Customer not found" } });
                }

                if (messageDto.date == default(DateTime))
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "The date field is required." } });
                }

                var salesRep = await _unitOfWork.UserManager.FindByEmailAsync(salesRepEmail);
                if (salesRep == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative not found" } });
                }




                var message = new Message
                {
                   
                    MessageContent = messageDto.summary,
                    MessageDate = messageDto.date,
                    FollowUpDate = messageDto.followUp,
                    SalesRepresntative = salesRep,
                    Customer = customer
                };

                await _unitOfWork.Messages.AddAsync(message);
                try
                {
                    _unitOfWork.complete();
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { e.Message } });
                }
               

                messages.Add(new MessageDto
                {
                    id = message.MessageID,
                    summary = message.MessageContent,
                    date = message.MessageDate,
                    followUp = message.FollowUpDate,
                    CustomerId = message.Customer.CustomerId
                });

                return new OkObjectResult(messages);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { ex.Message } });
            }
        }
        public async Task<ActionResult<IEnumerable<object>>> UpdateMessageInfo(updateMessagDto addMessageDto, string messageId)
        {

            var includes = new string[] { "Customer" };
            var message = await _unitOfWork.Messages.FindAsync(c => c.MessageID== messageId, includes);


            if (message == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Message not found" } });
            }


            if (addMessageDto.CustomerId.HasValue)
            {
                var newCustomer = await _unitOfWork.Customers.GetByIdAsync(addMessageDto.CustomerId.Value);

                if (newCustomer == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Customer not found" } });
                }

                message.Customer = newCustomer;
            }


            if (!string.IsNullOrEmpty(addMessageDto.summary))
            {
                message.MessageContent = addMessageDto.summary;
            }

            if (addMessageDto.date != default(DateTime))
            {
                message.MessageDate = addMessageDto.date;
            }

            if (addMessageDto.followUp != default(DateTime))
            {
                message.FollowUpDate = addMessageDto.followUp;
            }


            try
            {
                _unitOfWork.complete();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { ex.Message } });
            }


            var messageDto = new MessageDto
            {
                id = message.MessageID,
                summary = message.MessageContent,
               date =message.MessageDate,
               followUp =message.FollowUpDate,
                CustomerId = message.Customer != null ? message.Customer.CustomerId : 0,
            };


            return new OkObjectResult(new List<MessageDto> { messageDto });
        }
     

        public async Task<IEnumerable<MessageDto>> GetAllMessages()
        {
            try
            {
                var includes = new string[] { "Customer" };
                var messages = await _unitOfWork.Messages.GetAllAsync(message => true, int.MaxValue, 0, includes);

                if (messages == null || !messages.Any())
                {
                    return null ;
                }

                var Messages = messages.Select(message => new MessageDto
                {
                    id = message.MessageID,
                   // type = ActionType.message,
                    summary = message.MessageContent,
                    date = message.MessageDate,
                    followUp = message.FollowUpDate,
                    CustomerId = message.Customer != null ? message.Customer.CustomerId : 0
                });

                return Messages;
            }
            catch 
            {
               
                throw; 
            }
        }


        public async Task<IEnumerable<MessageDto>> GetMessageById(string messageID)
        {
            try
            {
                var includes = new string[] { "Customer" };
                var message = await _unitOfWork.Messages.FindAsync(c => c.MessageID == messageID, includes);

                if (message == null)
                {
                    return null;
                }

                var messageDto = new MessageDto
                {
                    id = message.MessageID,
                   // type = ActionType.message,
                    summary = message.MessageContent,
                    date = message.MessageDate,
                    followUp = message.FollowUpDate,
                    CustomerId = message.Customer != null ? message.Customer.CustomerId : 0
                };

                return new List<MessageDto> { messageDto };
            }
            catch 
            {
                
                throw; 
            }
        }


        public async Task<ResultDto> DeleteMessageById(string MessageId)
        {
            try
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

        public async Task<ActionResult<IEnumerable<object>>> AddMeeting(AddMeetingDto meetingDto, string SaleRepEmail)
        {
            try
            {
                if (meetingDto == null)
                {
                    throw new ArgumentNullException(nameof(MeetingDto));
                }

                var customer = await _unitOfWork.Customers.GetByIdAsync(meetingDto.CustomerId);
                if (customer == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Customer not found" } });
                }

                if (string.IsNullOrEmpty(SaleRepEmail))
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative email is null or empty" } });
                }

                var salesRep = await _unitOfWork.UserManager.FindByEmailAsync(SaleRepEmail);
                if (salesRep == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative not found" } });
                }

                var meeting = new Meeting
                {
                    connectionState = meetingDto.online,
                    SalesRepresntative = salesRep,
                    Customer = customer,
                    MeetingDate = meetingDto.date,
                    MeetingSummary = meetingDto.summary
                };

                // Set FollowUpDate to null if not provided in the request
                if (meetingDto.followUp == null)
                {
                    meeting.FollowUpDate = null;
                }
                else
                {
                    meeting.FollowUpDate = meetingDto.followUp;
                }

                await _unitOfWork.Meetings.AddAsync(meeting);
                var meetings = new List<object>();

                try
                {
                    _unitOfWork.complete();
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { e.Message } });
                }

                meetings.Add(new MeetingDto
                {
                    id = meeting.MeetingID,
                    online = meeting.connectionState,
                    summary = meeting.MeetingSummary,
                    date = meeting.MeetingDate,
                    followUp = meeting.FollowUpDate,
                    CustomerId = meeting.Customer.CustomerId
                });

                return new OkObjectResult(meetings);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { ex.Message } });
            }
        }

        //public async Task<ActionResult<IEnumerable<object>>> AddMeeting(AddMeetingDto meetingDto, string SaleRepEmail)
        //{
        //    try
        //    {
        //        if (meetingDto == null)
        //        {
        //            throw new ArgumentNullException(nameof(MeetingDto));
        //        }

        //        var customer = await _unitOfWork.Customers.GetByIdAsync(meetingDto.CustomerId);
        //        if (customer == null)
        //        {
        //            return new BadRequestObjectResult(new { errors = new List<string> { "Customer not found" } });

        //        }

        //        if (string.IsNullOrEmpty(SaleRepEmail))
        //        {
        //            return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative email is null or empty" } });
        //        }

        //        var salesRep = await _unitOfWork.UserManager.FindByEmailAsync(SaleRepEmail);

        //        if (salesRep == null)
        //        {
        //            return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative not found" } });

        //        }


        //        var meeting = new Meeting
        //        {
        //            connectionState = meetingDto.online,
        //            SalesRepresntative = salesRep,
        //            Customer = customer,
        //            FollowUpDate = meetingDto.followUp,
        //            MeetingDate = meetingDto.date,
        //            MeetingSummary = meetingDto.summary
        //        };

        //        await _unitOfWork.Meetings.AddAsync(meeting);
        //        var meetings = new List<object>();

        //        try
        //        {
        //            _unitOfWork.complete();
        //        }

        //        catch (Exception e)
        //        {
        //            return new BadRequestObjectResult(new { errors = new List<string> { e.Message } });
        //        }

        //        meetings.Add(new MeetingDto
        //        {
        //            id = meeting.MeetingID,
        //            online =meeting.connectionState,
        //            summary = meeting.MeetingSummary,
        //            date = meeting.MeetingDate,
        //            followUp = meeting.FollowUpDate,
        //            CustomerId = meeting.Customer.CustomerId
        //        });


        //        return new OkObjectResult(meetings);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BadRequestObjectResult(new { errors = new List<string> { ex.Message } });
        //    }
        //}


        public async Task<ActionResult<IEnumerable<object>>> UpdateMeetingInfo(UpdateMeetingDto meetingDto, string meetingId)
        {

            var includes = new string[] { "Customer" };
            var meeting = await _unitOfWork.Meetings.FindAsync(c => c.MeetingID == meetingId, includes);


            if (meeting == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Meeting not found" } });
            }


            if (meetingDto.CustomerId.HasValue)
            {
                var newCustomer = await _unitOfWork.Customers.GetByIdAsync(meetingDto.CustomerId.Value);

                if (newCustomer == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Customer not found" } });
                }

                meeting.Customer = newCustomer;
            }


            if (meetingDto.summary != null)
            {
                meeting.MeetingSummary = meetingDto.summary;
            }

            if (meetingDto.date != null)
            {
                meeting.MeetingDate = meetingDto.date;
            }

            if (meetingDto.followUp == null)
            {
                meeting.FollowUpDate = null;
            }
            else
            {
                meeting.FollowUpDate = meetingDto.followUp;
            }
            if (meetingDto.online != null)
            {
                meeting.connectionState = meetingDto.online;
            }


            try
            {
                _unitOfWork.complete();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { ex.Message } });
            }


            var result = new List<object>
        {
            new
            {
                    id = meeting.MeetingID,
                    online = meeting.connectionState,
                    summary = meeting.MeetingSummary,
                    date = meeting.MeetingDate,
                    followUp = meeting.FollowUpDate,
                    CustomerId = meeting.Customer != null ? meeting.Customer.CustomerId : 0
            }
        };

            return new OkObjectResult(result);
        }


        public async Task<IEnumerable<MeetingDto>> GetAllMeetings()
        {
            try
            {
                var includes = new string[] { "Customer" };
                var meetings = await _unitOfWork.Meetings.GetAllAsync(meetings => true, int.MaxValue, 0, includes);

                if (meetings == null || !meetings.Any())
                {
                    return Enumerable.Empty<MeetingDto>();
                }

                var Meetings = meetings.Select(meeting => new MeetingDto
                {
                    id = meeting.MeetingID,
                  //  type = ActionType.meeting,
                    online = meeting.connectionState,
                    summary = meeting.MeetingSummary,
                    date = meeting.MeetingDate,
                    followUp = meeting.FollowUpDate,
                    CustomerId = meeting.Customer != null ? meeting.Customer.CustomerId : 0 
                });

                return Meetings;
            }
            catch 
            {
                
                throw; 
            }
        }


        public async Task<IEnumerable<MeetingDto>> GetMeetingByID(string MeetingId)
        {
            try
            {
                var includes = new string[] { "Customer" };
                var meeting = await _unitOfWork.Meetings.FindAsync(c => c.MeetingID == MeetingId, includes);

                if (meeting == null)
                {
                    return Enumerable.Empty<MeetingDto>();
                }

                var meetingDto = new MeetingDto
                {
                    id = meeting.MeetingID,
                  //  type = ActionType.meeting,
                    online = meeting.connectionState,
                    summary = meeting.MeetingSummary,
                    date = meeting.MeetingDate,
                    followUp = meeting.FollowUpDate,
                    
                    CustomerId = meeting.Customer != null ? meeting.Customer.CustomerId : 0 
                };

                return new List<MeetingDto> { meetingDto };
            }
            catch 
            {
               
                throw; 
            }
        }

        public async Task<ResultDto> DeleteMeeting(string MeetingId)
        {
            try
            {
                var meeting = await _unitOfWork.Meetings.FindAsync(c => c.MeetingID == MeetingId);
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
        public async Task<ActionResult<IEnumerable<object>>> AddDeal(AddDealDto dealsDto, string salesRepEmail)
        {
            try
            {
                if (dealsDto == null)
                {
                    throw new ArgumentNullException(nameof(dealsDto));
                }

                var deals = new List<object>();

                var customer = await _unitOfWork.Customers.GetByIdAsync(dealsDto.CustomerId);
                if (customer == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Customer Not Found" } });

                }
                var interest = await _unitOfWork.Interests.GetByIdAsync(dealsDto.InterestId);
                if (interest == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Interest Not Found" } });
                }
                if (dealsDto.date == default(DateTime)) 
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "The date field is required." } });
                }
                if (dealsDto.price == 0.0) 
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "The price field is required." } });
                }

                if (string.IsNullOrEmpty(salesRepEmail))
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative email is null or empty" } });
                }

                var salesRep = await _unitOfWork.UserManager.FindByEmailAsync(salesRepEmail);
                if (salesRep == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Sales Representative not found " } });
                }
                var deal = new Deal
                {
                    DealDate = dealsDto.date,
                description = dealsDto.summary,
                Price = dealsDto.price,
               Interest = interest,
                SalesRepresntative = salesRep,
               Customer = customer,
               };
                await _unitOfWork.Deals.AddAsync(deal);

                try
                {
                    _unitOfWork.complete();
                }
                catch (Exception e)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { e.Message } });
                }
              

                deals.Add(new DealsDto
                {
                    id = deal.DealId,
                    price = deal.Price,
                    interest=new InterestDto
                    {
                        Id=interest.InterestID,
                        Name= interest.InterestName
                    },
                    summary = deal.description,
                    date = deal.DealDate,
                    CustomerId = deal.Customer.CustomerId
                });

                return new OkObjectResult(deals);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { ex.Message } });
            }
        }

        public async Task<ActionResult<IEnumerable<object>>> UpdateDealInfo(UpdateDealDto dealDto, string dealId)
        {

            var includes = new string[] { "Customer", "Interest" };
            var deal = await _unitOfWork.Deals.FindAsync(c => c.DealId == dealId, includes);


            if (deal == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Deal not found" } });
            }


            if (dealDto.CustomerId.HasValue)
            {
                var newCustomer = await _unitOfWork.Customers.GetByIdAsync(dealDto.CustomerId.Value);

                if (newCustomer == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Customer not found" } });
                }

                deal.Customer = newCustomer;
            }
            if (dealDto.InterestId.HasValue)
            {
                var newInterest = await _unitOfWork.Interests.GetByIdAsync(dealDto.InterestId.Value);
                if (newInterest == null)
                {
                    return new BadRequestObjectResult(new { errors = new List<string> { "Interest not found" } });
                }
                deal.Interest = newInterest;
            }

            if (!string.IsNullOrEmpty(dealDto.summary))
            {
                deal.description = dealDto.summary;
            }


            if (dealDto.date != null && dealDto.date != default(DateTime))
            {
                deal.DealDate = dealDto.date;
            }


            if (dealDto.price != 0.0)
            {
                deal.Price = dealDto.price;
            }
            try
            {
                _unitOfWork.complete();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { ex.Message } });
            }

            var result = new List<object>
             {
               new
               {
                   id = deal.DealId,
                   price = deal.Price,
                   interest = new InterestDto
                    {
                       Id= deal.Interest != null ? deal.Interest.InterestID : 0,
                       Name = deal.Interest != null ? deal.Interest.InterestName: null
                    },
                   summary = deal.description,
                   date = deal.DealDate,
                   customerId = deal.Customer != null ? deal.Customer.CustomerId : 0,
        }
            };

            return new OkObjectResult(result);
        }
           








            public async Task<IEnumerable<DealsDto>> GetAllDeals()
        {
            try
            {
                var includes = new string[] { "Customer", "Interest" };
                var deals = await _unitOfWork.Deals.GetAllAsync(call => true, int.MaxValue, 0, includes);

                if (deals == null || !deals.Any())
                {
                    return Enumerable.Empty<DealsDto>();
                }

                var Deals = deals.Select(deal => new DealsDto
                {
                    id = deal.DealId,
                    price = deal.Price,
                    interest = new InterestDto
                    {
                        Id = deal.Interest.InterestID,
                        Name = deal.Interest.InterestName
                    },
                    summary = deal.description,
                    date = deal.DealDate,
                    CustomerId=deal.Customer.CustomerId
                });

                return Deals;
            }
            catch
            {
                
                throw; 
            }
        }

        public async Task<IEnumerable<DealsDto>> GetDealById(string DealId)
        {
            try
            {
                var includes = new string[] { "Customer", "Interest" };
                var deal = await _unitOfWork.Deals.FindAsync(c => c.DealId == DealId, includes);

                if (deal == null)
                {
                    return Enumerable.Empty<DealsDto>();
                }

                var dealDto = new DealsDto
                {
                    id = deal.DealId,
                    price = deal.Price,
                    interest = new InterestDto
                    {
                        Id = deal.Interest.InterestID,
                        Name = deal.Interest.InterestName
                    },
                    summary = deal.description,
                    date = deal.DealDate,
                    CustomerId = deal.Customer.CustomerId
                };

                return new List<DealsDto> { dealDto };
            }
            catch 
            {
                
                throw; 
            }
        }

        public async Task<ResultDto> DeleteDeal(string dealId)
        {
            try
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

     
        public async Task<ActionResult<IEnumerable<object>>> GetAllActionsForCustomer(int customerId)
        {
            var salesRepresentativeEmail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var salesRepresentative = await _unitOfWork.UserManager.FindByEmailAsync(salesRepresentativeEmail);

            if (salesRepresentative == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Sales representative not found" } });
            }

            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);

            if (customer == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Customer not found" } });
            }

            if (customer.SalesRepresntative == null || customer.SalesRepresntative.Id != salesRepresentative.Id)
            {
                return new BadRequestObjectResult( new { errors = new List<string> { "This customer is not assigned to you" } });
            }

            var actions = await _sharedService.GetActionsForCustomer(customerId);

            return new OkObjectResult(actions);
        }


        public async Task<ActionResult<IEnumerable<ReturnAllCustomersDto>>> GetAllCustomersForSalesRep(int page, int size)
        {
            var salesRepresentativeEmail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);

            var salesRepresentative = await _unitOfWork.UserManager.FindByEmailAsync(salesRepresentativeEmail);

            if (salesRepresentative == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Sales representative not found" } });
            }


            var customers = await _unitOfWork.Customers.GetAllAsync(
                c => !c.IsDeleted && c.SalesRepresntative.Id == salesRepresentative.Id,
                ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]
            );

            if (customers == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Customers not found" } });
            }

            //if (customers.SingleOrDefault() == null || customers.SalesRepresntative.Id != salesRepresentative.Id)
            //{
            //    return new BadRequestObjectResult(new { errors = new List<string> { "This customer is not assigned to you" } });
            //}

            var customersDto = new List<ReturnCustomerDto>();
            foreach (var customer in customers)
            {
                var customerDto = new ReturnCustomerDto
                {
                    Id = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    City = customer.City,
                    Age = customer.Age,
                    Gender = customer.Gender,
                    AdditionDate = customer.AdditionDate
                };

                // Set source details
                if (customer.Source != null)
                {
                    customerDto.Source = new SourceDto
                    {
                        Id = customer.Source.SourceId,
                        Name = customer.Source.SourceName
                    };
                }

                // Set interests
                customerDto.Interests = customer.Interests.Select(i => new UserInterestDto { Id = i.InterestID, Name = i.InterestName }).ToList();

                // Set sales representative information
                var salesRepDto = new UserDto
                {
                    Id = salesRepresentative.Id,
                    FirstName = salesRepresentative.FirstName,
                    LastName = salesRepresentative.LastName,
                    UserName = salesRepresentative.UserName,
                    Roles = await _unitOfWork.UserManager.GetRolesAsync(salesRepresentative),
                    Email = salesRepresentative.Email
                };

                customerDto.SalesRepresentative = salesRepDto;
                var userdto2 = new UserDto
                {
                    Id = customer.MarketingModerator.Id,
                    FirstName = customer.MarketingModerator.FirstName,
                    LastName = customer.MarketingModerator.LastName,
                    UserName = customer.MarketingModerator.UserName,
                    Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.MarketingModerator),
                    Email = customer.MarketingModerator.Email
                };
                customerDto.AddedBy = userdto2;
               // customersDto.Add(customerDto);

                // Set last action
                var lastAction = await _sharedService.GetLastAction(customer.CustomerId);
                if (lastAction != null && lastAction.Summary != null)
                {
                    customerDto.LastAction = lastAction;
                }

                customersDto.Add(customerDto);
            }

            var paginatedCustomers = _filterService.Paginate(customersDto.OrderByDescending(c => c.AdditionDate).ToList(), page, size);

            return new OkObjectResult(paginatedCustomers);
        }



        public async Task<ActionResult<IEnumerable<object>>> GetCustomer(int customerId)
        {
            var salesRepresentativeEmail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var salesRepresentative = await _unitOfWork.UserManager.FindByEmailAsync(salesRepresentativeEmail);

            if (salesRepresentative == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Sales representative not found" } });
            }

            var customer = await _unitOfWork.Customers.FindAsync(c => c.CustomerId == customerId && !c.IsDeleted, ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);

            if (customer == null)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "Customer not found" } });
            }

            if (customer.SalesRepresntative == null || customer.SalesRepresntative.Id != salesRepresentative.Id)
            {
                return new BadRequestObjectResult(new { errors = new List<string> { "This customer is not assigned to you" } });
            }

            var customerDto = new ReturnCustomerDto();
            customerDto.IsSuccess = true;
            customerDto.Id = customer.CustomerId;
            customerDto.FirstName = customer.FirstName;
            customerDto.LastName = customer.LastName;
            customerDto.Email = customer.Email;
            customerDto.Phone = customer.Phone;
            customerDto.City = customer.City;
            customerDto.Age = customer.Age;
            customerDto.Gender = customer.Gender;
        
            if (customer.Source != null)
            {
                customerDto.Source = new SourceDto
                {
                    Id = customer.Source.SourceId,
                    Name = customer.Source.SourceName
                };
            }
            else
            {
                
                customerDto.Source = null; 
            }

            customerDto.AdditionDate = customer.AdditionDate;
            var interests = await _unitOfWork.Interests.GetAllAsync();
            if (interests == null)
            {
                customerDto.IsSuccess = false;
                customerDto.Errors = ["No interests found"];

                return new OkObjectResult(customerDto);
            }
            customerDto.Interests = new List<UserInterestDto>();

            customerDto.Interests = customer.Interests.Select(i => new UserInterestDto { Id = i.InterestID, Name = i.InterestName }).ToList();
            var lastAction = await _sharedService.GetLastAction(customer.CustomerId);
            if (lastAction.Summary != null)
            {
                customerDto.LastAction = lastAction;
            }
            var userdto = new UserDto
            {
                Id = customer.SalesRepresntative.Id,
                FirstName = customer.SalesRepresntative.FirstName,
                LastName = customer.SalesRepresntative.LastName,
                UserName = customer.SalesRepresntative.UserName,
                Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.SalesRepresntative),
                Email = customer.SalesRepresntative.Email,
                customers = await _unitOfWork.Customers.CountAsync(c => c.SalesRepresntative.Id == customer.SalesRepresntative.Id)
            };
            customerDto.SalesRepresentative = userdto;
            if (customer.MarketingModerator != null)
            {
                var userdto2 = new UserDto
                {
                    Id = customer.MarketingModerator.Id,
                    FirstName = customer.MarketingModerator.FirstName,
                    LastName = customer.MarketingModerator.LastName,
                    UserName = customer.MarketingModerator.UserName,
                    Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.MarketingModerator),
                    Email = customer.MarketingModerator.Email
                };

                customerDto.AddedBy = userdto2;
            }
            else
            {
                customerDto.AddedBy = null; 
            }

            return new OkObjectResult(customerDto);


        }

        public async Task<ReturnAllCustomersDto> Search(string query, int page, int size,string id)
        {
            //var customers = await _unitOfWork.Customers.GetAllAsync(c => c.FirstName.ToLower().Contains(query.ToLower()), ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            //if (!customers.Any())
            //{
            //    customers = await _unitOfWork.Customers.GetAllAsync(c => c.LastName.ToLower().Contains(query.ToLower()), ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            //}

            var user = await _usermanager.FindByIdAsync(id);
            var assignedCustomers = await _unitOfWork.Customers.GetAllAsync(c=>c.SalesRepresntative==user, ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);

            //var customers = await _unitOfWork.Customers.GetAllAsync(c => (c.FirstName.ToLower() + " " + c.LastName.ToLower()).Contains(query.ToLower()) && !c.IsDeleted&& c.SalesRepresntative == user);
            //if (!customers.Any())
            //{
            //    customers = await _unitOfWork.Customers.GetAllAsync(c => c.FirstName.ToLower().Contains(query.ToLower()) && !c.IsDeleted&&c.SalesRepresntative==user);
            //}
            //if (!customers.Any())
            //{
            //    customers = await _unitOfWork.Customers.GetAllAsync(c => c.LastName.ToLower().Contains(query.ToLower()) && !c.IsDeleted&&c.SalesRepresntative==user);
            //}
            //if (!customers.Any())
            //{
            //    customers = await _unitOfWork.Customers.GetAllAsync(c => c.Email.ToLower().Contains(query.ToLower()) && !c.IsDeleted&&c.SalesRepresntative==user);
            //}
            //if (!customers.Any())
            //{
            //    customers = await _unitOfWork.Customers.GetAllAsync(c => c.Phone.ToLower().Contains(query.ToLower()) && !c.IsDeleted&&c.SalesRepresntative==user);
            //}

            

            var customers = assignedCustomers.Where(c => (c.FirstName.ToLower() + " " + c.LastName.ToLower()).Contains(query.ToLower()) && !c.IsDeleted );
            if (!customers.Any())
            {
                customers = assignedCustomers.Where(c => c.FirstName.ToLower().Contains(query.ToLower()) && !c.IsDeleted );
            }
            if (!customers.Any())
            {
                customers = assignedCustomers.Where(c => c.LastName.ToLower().Contains(query.ToLower()) && !c.IsDeleted );
            }
            if (!customers.Any())
            {
                customers = assignedCustomers.Where(c => c.Email.ToLower().Contains(query.ToLower()) && !c.IsDeleted );
            }
            if (!customers.Any())
            {
                customers = assignedCustomers.Where(c => c.Phone.ToLower().Contains(query.ToLower()) && !c.IsDeleted );
            }

            var customerResult = new List<ReturnCustomerDto>();
            foreach (var customer in customers)
            {
                var dto = new ReturnCustomerDto
                {
                    Id = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Age = customer.Age,
                    City = customer.City,
                    Email = customer.Email,
                    Gender = customer.Gender,
                    Phone = customer.Phone,
                    //SourceId = customer.Source.SourceId,
                    //SalesRepresntativeId = customer.SalesRepresntative.Id,
                    //Interests = customer.Interests.Select(i => new UserInterestDto { /*Id = i.InterestID,*/ Name = i.InterestName, IsSelected = true }).ToList(),
                    AdditionDate = customer.AdditionDate
                };
                var lastAction = await _sharedService.GetLastAction(customer.CustomerId);
                if (lastAction.Summary != null)
                {
                    dto.LastAction = lastAction;
                }
                dto.Source = new SourceDto
                {
                    Id = customer.Source.SourceId,
                    Name = customer.Source.SourceName
                };
                //foreach (var interest in customer.Interests)
                //{
                //    dto.Interests.Add(new UserInterestDto { Id = interest.InterestID, Name = interest.InterestName });
                //}
                dto.Interests = customer.Interests.Select(i => new UserInterestDto { Id = i.InterestID, Name = i.InterestName }).ToList();

                if (customer.SalesRepresntative != null)
                {
                    var userdto = new UserDto
                    {
                        Id = customer.SalesRepresntative.Id,
                        FirstName = customer.SalesRepresntative.FirstName,
                        LastName = customer.SalesRepresntative.LastName,
                        UserName = customer.SalesRepresntative.UserName,
                        Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.SalesRepresntative),
                        Email = customer.SalesRepresntative.Email,
                        customers = await _unitOfWork.Customers.CountAsync(c => c.SalesRepresntative.Id == customer.SalesRepresntative.Id)
                    };
                    dto.SalesRepresentative = userdto;
                }
                if (customer.MarketingModerator != null)
                {
                    var userdto2 = new UserDto
                    {
                        Id = customer.MarketingModerator.Id,
                        FirstName = customer.MarketingModerator.FirstName,
                        LastName = customer.MarketingModerator.LastName,
                        UserName = customer.MarketingModerator.UserName,
                        Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.MarketingModerator),
                        Email = customer.MarketingModerator.Email
                    };
                    dto.AddedBy = userdto2;
                }


                customerResult.Add(dto);
            }
            var Customers = customerResult.OrderByDescending(DateTime => DateTime.AdditionDate).ToList();
            var CustomerPage = _filterService.Paginate(Customers, page, size);
            return new ReturnAllCustomersDto
            {
                IsSuccess = true,
                Pages = CustomerPage
                //Customers = customersDto.OrderByDescending(DateTime => DateTime.AdditionDate).ToList()
            };
        }


    }
}









    

