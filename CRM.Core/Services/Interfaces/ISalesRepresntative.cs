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
        Task<ResultDto> UpdateCallInfo(CallDto callDto, int CallId);
        Task<ReturnCallsDto> GetAllCalls();
        Task<ReturnCallsDto> GetCallById(int callId);
        Task<ResultDto> DeleteCallById(int callId);
        #endregion

        #region ManageMessages
        Task<ResultDto> AddMessage(MessageDto MessageDto, string SalesRepresntativeEmail);
        Task<ResultDto> UpdateMessageInfo(MessageDto messageDto, int MessageId);
        Task<ReturnMessagesDto> GetAllMessages();
        Task<ReturnMessagesDto> GetMessageById(int MessageId);
        Task<ResultDto> DeleteMessageById(int MessageId);

        #endregion

        #region ManageMeetings
        Task<ResultDto> AddMeeting(MeetingDto meetingDto, string SaleRepEmail);
        Task<ResultDto> UpdateMeeting(MeetingDto meetingDto, int MeetingId);
        Task<ReturnMeetingsDto> GetAllMeetings();
        Task<ReturnMeetingsDto> GetMeetingByID(int MeetingId);
        Task<ResultDto> DeleteMeeting(int MeetingId);
        #endregion

        #region ManageDeals
        Task<ResultDto> AddDeal(DealsDto dealsDto, string SaleRepEmail);
        Task<ResultDto> UpdateDeal(DealsDto dealsDto, int DealId);
        Task<ReturnDealsDto> GetAllDeals();
        Task<ReturnDealsDto> GetDealById(int DealId);
        Task<ResultDto> DeleteDeal(int DealId);

        #endregion

    }
}
