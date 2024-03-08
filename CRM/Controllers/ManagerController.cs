using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRM.Controllers
{
    [Authorize(Roles = "Manager")]
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
        [HttpGet("get-all-roles")]
        public IActionResult GetAllRoles()
        {
            var result = _managerService.GetAllRoles();
            return Ok(result);
        }
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _managerService.GetAllUsers();
            return Ok(result);
        }
        [HttpGet("user-roles/{userId}")]
        public async Task<IActionResult> ViewUserRoles(string userId)
        {
            var result = await _managerService.ViewUserRoles(userId);
            if(result.Errors != null)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpPut("update-user-roles")]
        public async Task<IActionResult> UpdateUserRoles(UserRolesDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _managerService.ManageUserRoles(dto);
            if(!result.IsSucces)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
    }
}
