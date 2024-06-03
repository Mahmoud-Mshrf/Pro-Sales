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
            try
            {
                var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
                var result = await _salesRepresntative.AddCall(callDto, SalesRepresntativeEmail);

                if (result.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (result.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }


                return StatusCode(500, "An unexpected error occurred");
            }
            catch
            {

                return StatusCode(500, "Internal server error");
            }
        }




        [HttpPut("[action]/{callId}")]
        public async Task<IActionResult> UpdateCallInfo(string callId, [FromBody] UpdateCallDto callDto)
        {
            try
            {
                var salesRepresentativeEmail = User.FindFirstValue(ClaimTypes.Email);
                var result = await _salesRepresntative.UpdateCallInfo(callDto, callId);

                if (result.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (result.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }

                return StatusCode(500, "An unexpected error occurred");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }





        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCalls()
        {
            try
            {
                var calls = await _salesRepresntative.GetAllCalls();
                if (calls == null || !calls.Any())
                {
                    return NotFound(new { errors = new[] { " No Calls found" } });
                }

                return Ok(calls);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("[action]/{callId}")]
        public async Task<IActionResult> GetCallById(string callId)
        {
            try
            {
                var call = await _salesRepresntative.GetCallById(callId);
                if (call == null || !call.Any())
                {
                    return NotFound(new { errors = new[] { "Call not found" } });
                }

                return Ok(call);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
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
        public async Task<IActionResult> AddMessage([FromBody] AddMessageDto messageDto)
        {
            try
            {
                var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
                var result = await _salesRepresntative.AddMessage(messageDto, SalesRepresntativeEmail);

                if (result.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (result.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }


                return StatusCode(500, "An unexpected error occurred");
            }
            catch
            {

                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("[action]/{messageId}")]
        public async Task<IActionResult> UpdateMessageInfo(string messageId, [FromBody] updateMessagDto messageDto)
        {
            try
            {
                var salesRepresentativeEmail = User.FindFirstValue(ClaimTypes.Email);
                var result = await _salesRepresntative.UpdateMessageInfo(messageDto, messageId);

                if (result.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (result.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }

                return StatusCode(500, "An unexpected error occurred");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllMessages()
        {
            try
            {
                var messages = await _salesRepresntative.GetAllMessages();
                if (messages == null || !messages.Any())
                {
                    return NotFound(new { errors = new[] { " No Messages found" } });
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("[action]/{messageId}")]
        public async Task<IActionResult> GetMessageById(string messageId)
        {
            try
            {
                var message = await _salesRepresntative.GetMessageById(messageId);
                if (message == null || !message.Any())
                {
                    return NotFound(new { errors = new[] { "Message not found" } });
                }

                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
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
            try
            {
                var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
                var result = await _salesRepresntative.AddMeeting(meetingDto, SalesRepresntativeEmail);

                if (result.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (result.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }


                return StatusCode(500, "An unexpected error occurred");
            }
            catch
            {

                return StatusCode(500, "Internal server error");
            }
        }



        [HttpPut("[action]/{MeetingId}")]
        public async Task<IActionResult> UpdateMeetingInfo(string meetingId, [FromBody] UpdateMeetingDto meetingDto)
        {
            try
            {
                var salesRepresentativeEmail = User.FindFirstValue(ClaimTypes.Email);
                var result = await _salesRepresntative.UpdateMeetingInfo(meetingDto, meetingId);

                if (result.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (result.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }

                return StatusCode(500, "An unexpected error occurred");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }








        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllMeetings()
        {
            try
            {
                var meetings = await _salesRepresntative.GetAllMeetings();
                if (meetings == null || !meetings.Any())
                {
                    return NotFound(new { errors = new[] { " No Meetings found" } });
                }

                return Ok(meetings);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("[action]/{meetingId}")]
        public async Task<IActionResult> GetMeetingById(string meetingId)
        {
            try
            {
                var meeting = await _salesRepresntative.GetMeetingByID(meetingId);
                if (meeting == null || !meeting.Any())
                {
                    return NotFound(new { errors = new[] { "Meeting not found" } });
                }

                return Ok(meeting);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
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
            try
            {
                var SalesRepresntativeEmail = User.FindFirstValue(ClaimTypes.Email);
                var result = await _salesRepresntative.AddDeal(dealDto, SalesRepresntativeEmail);

                if (result.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (result.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }


                return StatusCode(500, "An unexpected error occurred");
            }
            catch
            {

                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("[action]/{DealId}")]
        public async Task<IActionResult> UpdateDealInfo(string dealId, [FromBody] UpdateDealDto dealDto)
        {
            try
            {
                var salesRepresentativeEmail = User.FindFirstValue(ClaimTypes.Email);
                var result = await _salesRepresntative.UpdateDealInfo(dealDto, dealId);

                if (result.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (result.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }

                return StatusCode(500, "An unexpected error occurred");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllDeals()
        {
            try
            {
                var deals = await _salesRepresntative.GetAllDeals();
                if (deals == null || !deals.Any())
                {
                    return NotFound(new { errors = new[] { " No Deals found" } });
                }

                return Ok(deals);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("[action]/{dealId}")]
        public async Task<IActionResult> GetDealById(string dealId)
        {
            try
            {
                var deal = await _salesRepresntative.GetDealById(dealId);
                if (deal == null || !deal.Any())
                {
                    return NotFound(new { errors = new[] { "Deal not found" } });
                }

                return Ok(deal);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
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
            catch
            {

                return StatusCode(500, "Internal server error");
            }


        }
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomersForSalesRep(int page = 1, int size = 10, [FromQuery] string? query = null)
        {
            try
            {
                if (query!=null)
                {
                    var id = User.FindFirstValue("id");
                    var resultt = await _salesRepresntative.Search(query, page, size, id);
                    if (!resultt.IsSuccess)
                    {
                        return BadRequest(resultt);
                    }
                    return Ok(resultt.Pages);
                }
                var result = await _salesRepresntative.GetAllCustomersForSalesRep(page, size);

                if (result.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }
                if (result.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }


                return StatusCode(500, "An unexpected error occurred");
            }
            catch
            {

                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetCustomerAssignedToSales/{customerId}")]
        public async Task<IActionResult> GetCustomerAssignedToSales(int customerId)
        {
            try
            {
                var Result = await _salesRepresntative.GetCustomer(customerId);

                if (Result.Result is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (Result.Result is OkObjectResult okResult)
                {
                    return Ok(okResult.Value);
                }


                return StatusCode(500, "An unexpected error occurred");
            }
            catch
            {

                return StatusCode(500, "Internal server error");
            }


        }

        [HttpGet("search-assigned-customers")]
        public async Task<IActionResult> SearchAssingedCustomers(int page, int size, [FromQuery] string? query)
        {
            var id = User.FindFirstValue("id");
            var result = await _salesRepresntative.Search(query, page, size,id);
            return Ok(result.Pages);
        }











    }
}
