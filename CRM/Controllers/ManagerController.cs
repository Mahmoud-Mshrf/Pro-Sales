using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            if (string.IsNullOrEmpty(result.Name))
            {
                var errors =new { errors = new string[] { $"Interest already exists" } };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpPut("update-interest")]
        public async Task<IActionResult> UpdateInterest([FromBody] InterestDto dto)
        {
            var result = await _managerService.updateInterest(dto);
            if (!result.IsSuccess)
            {
                var errors = new { result.Errors};
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpGet("get-interest/{id}")]
        public async Task<IActionResult> GetInterest(int id)
        {
            var result = await _managerService.getInterest(id);
            if (string.IsNullOrEmpty(result.Name))
            {
                var errors = new { errors = new string[] { "Interest not found" } };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        //[HttpPatch("disable-interest/{id}")]
        //public async Task<IActionResult> DisableInterest(int id)
        //{
        //    var result = await _managerService.DisableInterest(id);
        //    if (!result.IsSuccess)
        //    {
        //        var errors = new string[] {"Interest not found"};
        //        return BadRequest();
        //    }
        //    return Ok(result);
        //}
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
            if (!result.IsSucces)
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
            if (!result.IsSucces)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        //[HttpPost("add-business-info")]
        [HttpPut("update-business-info")]
        public async Task<IActionResult> UpdateBusinessInfo([FromBody] BusinessDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _managerService.UpdateBusinessInfo(email, dto);
            return Ok(result);
        }
    }
}
