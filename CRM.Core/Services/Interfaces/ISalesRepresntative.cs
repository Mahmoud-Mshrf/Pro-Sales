using CRM.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Interfaces
{
    public interface ISalesRepresntative
    {
        #region MangeCalls
        
        Task<ActionResult<IEnumerable<object>>> AddCall(AddCallDto addcallDto, string salesRepEmail);
        // Task<ReturnCallsDto> UpdateCallInfo(AddCallDto callDto, string CallId);
        Task<List<CallDto>> UpdateCallInfo(AddCallDto addCallDto, string callId);
        Task<ReturnCallsDto> GetAllCalls();
        Task<ReturnCallsDto> GetCallById(string callId);
        Task<ResultDto> DeleteCallById(string callId);
        #endregion

        #region ManageMessages
        Task<ActionResult<IEnumerable<object>>> AddMessage(AddMessageDto messageDto, string salesRepEmail);
        Task<ResultDto> UpdateMessageInfo(AddMessageDto messageDto, string MessageId);
        Task<ReturnMessagesDto> GetAllMessages();
        Task<ReturnMessagesDto> GetMessageById(string MessageId);
        Task<ResultDto> DeleteMessageById(string MessageId);

        #endregion

        #region ManageMeetings
        Task<ActionResult<IEnumerable<object>>> AddMeeting(AddMeetingDto meetingDto, string SaleRepEmail);
        Task<ResultDto> UpdateMeeting(AddMeetingDto meetingDto, string MeetingId);
        Task<ReturnMeetingsDto> GetAllMeetings();
        Task<ReturnMeetingsDto> GetMeetingByID(string MeetingId);
        Task<ResultDto> DeleteMeeting(string MeetingId);
        #endregion

        #region ManageDeals
        Task<ActionResult<IEnumerable<object>>> AddDeal(AddDealDto addcallDto, string salesRepEmail);

        Task<ResultDto> UpdateDeal(AddDealDto dealsDto, string DealId);
        Task<ReturnDealsDto> GetAllDeals();
        Task<ReturnDealsDto> GetDealById(string DealId);
        Task<ResultDto> DeleteDeal(string DealId);


        #endregion

        
        Task<ActionResult<IEnumerable<object>>> GetAllActionsForCustomer(int customerId);

    }
}
