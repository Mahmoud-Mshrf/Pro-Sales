using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM.Controllers
{
    [Authorize(Roles = "Sales Representative,Marketing Moderator")] // later will be [Authorize(Roles ="MarketingModerator")]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesRepController : ControllerBase
    {


        private readonly ISalesRepresntative _salesRepresntative;
        public SalesRepController(ISalesRepresntative salesRepresntative)
        {
            _salesRepresntative = salesRepresntative;
        }

        #region Manage Calls
        [HttpPost("[action]")]
        public async Task<IActionResult> AddCall([FromBody] CallDto callDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _salesRepresntative.AddCall(callDto, SalesRepresntativeEmail);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateCall([FromBody] CallDto callDto, int CallId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _salesRepresntative.UpdateCallInfo(callDto, CallId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCalls()
        {
            var result = await _salesRepresntative.GetAllCalls();
            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Calls);
        }

        [HttpGet("[action]/{callId}")]
        public async Task<IActionResult> GetCallById(int callId)
        {
            var result = await _salesRepresntative.GetCallById(callId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Errors);
            }
            return Ok(result);
        }
        [HttpDelete("[action]/{callId}")]
        public async Task<IActionResult> DeleteCallById(int callId)
        {
            var result = await _salesRepresntative.DeleteCallById(callId);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        #endregion

        #region ManageMessages
        [HttpPost("[action]")]
        public async Task<IActionResult> AddMessages([FromBody] MessageDto messageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _salesRepresntative.AddMessage(messageDto, SalesRepresntativeEmail);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateMessage([FromBody] MessageDto messageDto, int MessageId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _salesRepresntative.UpdateMessageInfo(messageDto, MessageId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllMessages()
        {
            var result = await _salesRepresntative.GetAllMessages();
            if (!result.IsSuccess)
                return BadRequest(result.Errors);
            return Ok(result.Messages);
        }

        [HttpGet("[action]/{messageId}")]
        public async Task<IActionResult> GetMessageById(int MessageId)
        {
            var result = await _salesRepresntative.GetMessageById(MessageId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Errors);
            }
            return Ok(result.Messages);
        }

        [HttpDelete("[action]/{MessageId}")]
        public async Task<IActionResult> DeleteMessageById(int messageId)
        {
            var result = await _salesRepresntative.DeleteMessageById(messageId);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }



        #endregion

        #region ManageMeetings 



        [HttpPost("[action]")]
        public async Task<IActionResult> AddMeeting([FromBody] MeetingDto meetingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var SalesRepEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _salesRepresntative.AddMeeting(meetingDto, SalesRepEmail);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateMeeting([FromBody] MeetingDto meetingDto, int MeetingId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _salesRepresntative.UpdateMeeting(meetingDto, MeetingId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllMeetings()
        {
            var result = await _salesRepresntative.GetAllMeetings();
            if (!result.IsSuccess)
                return BadRequest(result.Errors);
            return Ok(result.Meetings);
        }

        [HttpGet("[action]/{meetingId}")]
        public async Task<IActionResult> GetMeetingById(int MeetingId)
        {
            var result = await _salesRepresntative.GetMeetingByID(MeetingId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Errors);
            }
            return Ok(result);
        }

        [HttpDelete("[action]/{MeetingId}")]
        public async Task<IActionResult> DeleteMeetingById(int meetingId)
        {
            var result = await _salesRepresntative.DeleteMeeting(meetingId);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }


        #endregion

        #region ManageDeals
        [HttpPost("[action]")]
        public async Task<IActionResult> AddDeal([FromBody] DealsDto dealDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _salesRepresntative.AddDeal(dealDto, SalesRepresntativeEmail);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateDeal([FromBody] DealsDto dealDto, int DealId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _salesRepresntative.UpdateDeal(dealDto, DealId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllDeals()
        {
            var result = await _salesRepresntative.GetAllDeals();
            if (!result.IsSuccess)
                return BadRequest(result.Errors);
            return Ok(result.Deals);
        }

        [HttpGet("[action]/{dealId}")]
        public async Task<IActionResult> GetDealById(int dealId)
        {
            var result = await _salesRepresntative.GetDealById(dealId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Errors);
            }
            return Ok(result);
        }
        [HttpDelete("[action]/{dealId}")]
        public async Task<IActionResult> DeleteDealById(int dealId)
        {
            var result = await _salesRepresntative.DeleteDeal(dealId);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        #endregion




    }
}
