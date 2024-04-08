using CRM.Core.Dtos;
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
        Task<ResultDto> AddCall(CallDto callDto, string SalesRepresntativeEmail);
        Task<ResultDto> UpdateCallInfo(CallDto callDto, string CallId);
        Task<ReturnCallsDto> GetAllCalls();
        Task<ReturnCallsDto> GetCallById(string callId);
        Task<ResultDto> DeleteCallById(string callId);
        #endregion

        #region ManageMessages
        Task<ResultDto> AddMessage(MessageDto MessageDto, string SalesRepresntativeEmail);
        Task<ResultDto> UpdateMessageInfo(MessageDto messageDto, string MessageId);
        Task<ReturnMessagesDto> GetAllMessages();
        Task<ReturnMessagesDto> GetMessageById(string MessageId);
        Task<ResultDto> DeleteMessageById(string MessageId);

        #endregion

        #region ManageMeetings
        Task<ResultDto> AddMeeting(MeetingDto meetingDto, string SaleRepEmail);
        Task<ResultDto> UpdateMeeting(MeetingDto meetingDto, string MeetingId);
        Task<ReturnMeetingsDto> GetAllMeetings();
        Task<ReturnMeetingsDto> GetMeetingByID(string MeetingId);
        Task<ResultDto> DeleteMeeting(string MeetingId);
        #endregion

        #region ManageDeals
        Task<ResultDto> AddDeal(DealsDto dealsDto, string SaleRepEmail);
        Task<ResultDto> UpdateDeal(DealsDto dealsDto, string DealId);
        Task<ReturnDealsDto> GetAllDeals();
        Task<ReturnDealsDto> GetDealById(string DealId);
        Task<ResultDto> DeleteDeal(string DealId);


        #endregion
       
    }
}
