﻿using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAuthService _authService;
        public UserProfileController(IUserProfileService userProfileService, IAuthService authService)
        {
            _userProfileService = userProfileService;
            _authService = authService;
        }
        [HttpPut("update-name")]
        public async Task<IActionResult> UpdateName([FromBody] UpdateNameDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _userProfileService.UpdateNameAsync(email, dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _userProfileService.UpdatePasswordAsync(email, dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("update-email")]
        public async Task<IActionResult> UpdateEmail([FromBody] EmailDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _userProfileService.UpdateEmailAsync(email, dto.Email);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut("update-username")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _userProfileService.UpdateUsername(email, dto.Username);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpDelete("delete-my-account")]
        public async Task<IActionResult> DeleteMyAccount([FromBody] PasswordDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _userProfileService.DeleteMyAccount(email, dto.Password);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return NoContent();
        }
    }
}
