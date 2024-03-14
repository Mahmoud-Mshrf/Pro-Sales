using CRM.Core.Dtos;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
        [HttpPost("register")]
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



        //[HttpPost("confirm-email")]
        //public async Task<IActionResult> ConfirmEmail(VerifyCodeDto codeDto)
        //{
        //    var result = await _authService.ConfirmEmailAsync(codeDto);
        //    if (!result.IsAuthenticated)
        //    {
        //        var errors = new { errors = result.Errors };
        //        return BadRequest(errors);
        //    }
        //    if (!string.IsNullOrEmpty(result.RefreshToken))
        //    {
        //        setRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
        //    }
        //    return Ok(result);
        //}

        [HttpPost("login")]
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

        [HttpGet("refresh-token")]
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

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.RevokeToken(refreshToken);
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(-1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/; SameSite=None; Secure; Partitioned;"
                };
                Request.HttpContext.Response.Cookies.Delete("refreshToken", cookieOptions);
                return NoContent();
            }
            return NoContent();
        }


        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ForgotPasswordAsync(dto.Email);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("verify-code")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(VerifyCodeDto codeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (codeDto.Purpose == "ResetPassword")
            {
                var result = await _authService.VerifyCodeAsync(codeDto);
                if (result.IsSuccess)
                {
                    return Ok(result);

                }
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            else if (codeDto.Purpose == "ConfirmEmail")
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
            else if (codeDto.Purpose == "ConfirmNewEmail")
            {
                var result = await _authService.ConfirmNewEmailAsync(codeDto);
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
            else
            {
                var errors = new { errors = new string[] { "Invalid Purpose" } };
                return BadRequest(errors);
            }
            //var result = await _authService.VerifyCodeAsync(codeDto);
            //if (result.IsSuccess)
            //{
            //    return Ok(result);

            //}
            //return NotFound(result);
        }

        [HttpPost("reset-password")]
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
        [AllowAnonymous]
        [EnableCors("AllowAnyOrigin")] // Apply a different CORS policy for this endpoint
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("test-endpoint")]
        public IActionResult TestEndpoint()
        {
            return Ok("Test Endpoint");
        }
    }
}
