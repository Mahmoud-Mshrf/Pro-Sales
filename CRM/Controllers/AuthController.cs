﻿using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //if (!ModelState.IsValid)
            //{
            //    // Handle model state errors
            //    var errors = ModelState.ToDictionary(
            //        kvp => kvp.Key,
            //        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            //    );
            //    var modelStateErrorResponse = new { errors };
            //    return BadRequest(modelStateErrorResponse);
            //}
            var result = await _authService.RegisterAsync(dto);
            if (!result.IsSuccess)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }


        
        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmEmail(VerifyCodeDto codeDto)
        {
            var result = await _authService.ConfirmEmailAsync(codeDto);
            if (!result.IsAuthenticated)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] TokenRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.GetTokenAsync(dto);
            if (!result.IsAuthenticated)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }
            return Ok(result);
        }

        private void setRefreshTokenInCookie(string refreshToken, DateTime refreshTokenExpiration)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshTokenExpiration.ToLocalTime(),
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/; SameSite=None; Secure; Partitioned;"
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
                

            setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RevokeToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Token Is Required");
            var result = await _authService.RevokeToken(refreshToken);
            if (!result)
                return BadRequest("Invalid Token");
            return Ok("Token Revoked");
        }


        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromForm] string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                return NotFound(Email);
            }
            var result = await _authService.ForgotPasswordAsync(Email);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("VerifyCode")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(VerifyCodeDto codeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.VerifyCodeAsync(codeDto);
            if (result.IsSuccess)
            {
                return Ok(result);

            }
            return NotFound(result);
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ResetPasswordAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }
    }
}
