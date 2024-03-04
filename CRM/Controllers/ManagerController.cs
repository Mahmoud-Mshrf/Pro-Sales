using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpPost("add-interest")]
        public async Task<IActionResult> AddInterest([FromBody] NameDto dto)
        {
            var result = await _managerService.AddInterest(dto.Name);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("add-source")]
        public async Task<IActionResult> AddSource([FromBody] NameDto dto)
        {
            var result = await _managerService.AddSource(dto.Name);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
