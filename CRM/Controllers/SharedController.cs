using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SharedController : ControllerBase
    {
        private readonly ISharedService _sharedService;
        public SharedController(ISharedService sharedService)
        {
            _sharedService = sharedService;
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllInterests()
        {
            var result = await _sharedService.GetAllInterests();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result.Interests);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllSources()
        {
            var result = await _sharedService.GetAllSources();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result.Sources);
        }
    }
}
