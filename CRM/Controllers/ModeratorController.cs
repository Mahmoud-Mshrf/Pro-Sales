using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM.Controllers
{
    [Authorize] // later will be [Authorize(Roles ="MarketingModerator")]
    [Route("api/[controller]")]
    [ApiController]
    public class ModeratorController : ControllerBase
    {
        private readonly IModeratorService _moderatorService;
        public ModeratorController(IModeratorService moderatorService)
        {
            _moderatorService = moderatorService;
        }

        // Will be used after adding Manager module
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllSalesRepresentatives()
        {
            var result = await _moderatorService.GetAllSalesRepresentatives();
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Users);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var marketingModeratorEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _moderatorService.AddCustomer(customerDto,marketingModeratorEmail);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDto customerDto,int CustomerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var marketingModeratorEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = await _moderatorService.UpdateCustomer(customerDto, CustomerId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }

        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetAllSources()
        //{
        //    var result = await _moderatorService.GetAllSources();
        //    if(!result.IsSuccess)
        //        return BadRequest(result.Message);
        //    return Ok(result.Sources);
        //}
        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetAllInterests()
        //{
        //    var result = await _moderatorService.GetAllInterests();
        //    if (!result.IsSuccess)
        //        return BadRequest(result.Message);
        //    return Ok(result.Interests);
        //}

    }
}
