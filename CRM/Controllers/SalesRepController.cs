using CRM.Core;
using CRM.Core.Dtos;
using CRM.Core.Services.Implementations;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace CRM.Controllers
{
    [Authorize(Roles = "Sales Representative,Marketing Moderator")] // later will be [Authorize(Roles ="MarketingModerator")]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesRepController : ControllerBase
    {


        private readonly ISalesRepresntative _salesRepresntative;
        private readonly IUnitOfWork _unitOfWork;
        public SalesRepController(ISalesRepresntative salesRepresntative, IUnitOfWork unitOfWork)
        {
            _salesRepresntative = salesRepresntative;
            _unitOfWork = unitOfWork;
        }

        #region Manage Calls
        [HttpPost("[action]")]
        public async Task<IActionResult> AddCall([FromBody] AddCallDto callDto)
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
        public async Task<IActionResult> UpdateCall([FromBody] AddCallDto callDto, string CallId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _salesRepresntative.UpdateCallInfo(callDto, CallId);
            //if (!result.IsSuccess)
            //{
            //    return BadRequest(result);
            //}
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
        public async Task<IActionResult> GetCallById(string callId)
        {
            var result = await _salesRepresntative.GetCallById(callId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Errors);
            }
            return Ok(result);
        }
        [HttpDelete("[action]/{callId}")]
        public async Task<IActionResult> DeleteCallById(string callId)
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
        public async Task<IActionResult> AddMessages([FromBody] AddMessageDto messageDto)
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
        public async Task<IActionResult> UpdateMessage([FromBody] AddMessageDto messageDto, string MessageId)
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
        public async Task<IActionResult> GetMessageById(string MessageId)
        {
            var result = await _salesRepresntative.GetMessageById(MessageId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Errors);
            }
            return Ok(result.Messages);
        }

        [HttpDelete("[action]/{MessageId}")]
        public async Task<IActionResult> DeleteMessageById(string messageId)
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
        public async Task<IActionResult> AddMeeting([FromBody] AddMeetingDto meetingDto)
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
        public async Task<IActionResult> UpdateMeeting([FromBody] AddMeetingDto meetingDto, string MeetingId)
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
        public async Task<IActionResult> GetMeetingById(string MeetingId)
        {
            var result = await _salesRepresntative.GetMeetingByID(MeetingId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Errors);
            }
            return Ok(result);
        }

        [HttpDelete("[action]/{MeetingId}")]
        public async Task<IActionResult> DeleteMeetingById(string meetingId)
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
        public async Task<IActionResult> AddDeal([FromBody] AddDealDto dealDto)
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
        public async Task<IActionResult> UpdateDeal([FromBody] AddDealDto dealDto, string DealId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _salesRepresntative.UpdateDeal(dealDto,  DealId);
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
        public async Task<IActionResult> GetDealById(string dealId)
        {
            var result = await _salesRepresntative.GetDealById(dealId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Errors);
            }
            return Ok(result);
        }
        [HttpDelete("[action]/{dealId}")]
        public async Task<IActionResult> DeleteDealById(string dealId)
        {
            var result = await _salesRepresntative.DeleteDeal(dealId);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        #endregion
     
        [HttpGet("ActionsForCustomersAssignedToSales/{customerId}")]
        public async Task<IActionResult> GetCustomerActions(int customerId)
        {
            try
            {
                var actionResult = await _salesRepresntative.GetAllActionsForCustomer(customerId);

                if (actionResult.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (actionResult.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }

                
                return StatusCode(500, "An unexpected error occurred");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal server error");
            }
        }













    }
}
