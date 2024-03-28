using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Authorize(Roles = "Sales Representative,Marketing Moderator,Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class SharedController : ControllerBase
    {
        private readonly ISharedService _sharedService;
        public SharedController(ISharedService sharedService)
        {
            _sharedService = sharedService;
        }
        [AllowAnonymous]
        [Authorize]
        [HttpGet("get-all-interests")]
        public async Task<IActionResult> GetAllInterests()
        {
            var result = await _sharedService.GetAllInterests();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result.Interests);
        }
        [HttpGet("get-all-sources")]
        public async Task<IActionResult> GetAllSources()
        {
            var result = await _sharedService.GetAllSources();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result.Sources);
        }
        [AllowAnonymous]
        [Authorize]
        [HttpGet("get-business-info")]
        public async Task<IActionResult> GetBusinessInfo()
        {
            var result = await _sharedService.GetBussinesInfo();
            if (string.IsNullOrEmpty(result.CompanyName))
            {
                var errors = new { errors = new string[] { "The business information has not been added yet." } };
                return BadRequest(errors);
            }
            return Ok(result);
        }
    }
}
