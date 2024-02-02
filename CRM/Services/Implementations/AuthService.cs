﻿using CRM.Dtos;
using CRM.Helpers;
using CRM.Models;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CRM.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private readonly IMailingService _mailingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, IMailingService mailingService, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _mailingService = mailingService;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }




        // This method is used to generate a new Access Token for the user (will be called by Login Endpoint)
        public async Task<AuthModel> GetTokenAsync(TokenRequestDto dto)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null|| !await _userManager.CheckPasswordAsync(user,dto.Password))
            {
                authModel.Message = "Invalid Credentials";
                return authModel;
            }
            if(!user.EmailConfirmed)
            {
                authModel.Message = "Email not confirmed";
                return authModel;
            }
            var JwtToken = await CreateToken(user);
            var token = new JwtSecurityTokenHandler().WriteToken(JwtToken);
            authModel.AccessToken = token;
            authModel.IsAuthenticated = true;
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.Roles = JwtToken.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
            
            if(user.RefreshTokens.Any(x => x.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.Where(x => x.IsActive).FirstOrDefault();
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
            }
            return authModel;
        }

        public async Task<JwtSecurityToken> CreateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(_jwt.DurationInDays),
            signingCredentials: credentials
            );
            return token;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.Now.AddDays(10),
                CreatedOn = DateTime.Now
            };
        }


        public async Task<AuthModel> RefreshTokenAsync(string token)// this method is used to revoke the current refresh token and return a new refresh token 
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user is null)
            {
                authModel.Message = "Invalid Token";
                return authModel;
            }
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive)
            {
                authModel.Message = "Inactive Token";
                return authModel;
            }
            refreshToken.RevokedOn = DateTime.Now;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var JwtToken = await CreateToken(user);
            authModel.AccessToken = new JwtSecurityTokenHandler().WriteToken(JwtToken);
            authModel.IsAuthenticated = true;
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.Roles = JwtToken.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
            return authModel;

        }

        public async Task<bool> RevokeToken(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user is null)
                return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive)
                return false;
            refreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;

        }

        public async Task<ResultDto> RegisterAsync(RegisterDto dto)
        {
            if(await _userManager.FindByEmailAsync(dto.Email) is not null)
                return new ResultDto() { Message = "Email is already registered"};
            if(await _userManager.FindByNameAsync(dto.UserName) is not null)
                return new ResultDto{ Message = "UserName is already registered"};
            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.UserName,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                EmailConfirmed= false
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new ResultDto { Message = errors };
            }
            var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var callBackUrl = urlHelper.Action("ConfirmEmail", "Auth", new {Id = user.Id, Token = validEmailToken }, _httpContextAccessor.HttpContext.Request.Scheme);
            var message = new MailDto
            {
                MailTo = user.Email,
                Subject = "Confirm Your Email",
                Content = $"<h1>Welcome to CRM</h1> <p>Please confirm your email by <a href='{callBackUrl}'>Clicking here</a></p>",
            };
            var mailResult = await _mailingService.SendEmailAsync(message.MailTo,message.Subject,message.Content);
            if (mailResult)
                return new ResultDto { IsSuccess = true, Message = "Confirmation Email Sent, Please confirm your email" };
            else
                return new ResultDto { Message = "Email Confirmation Failed to send" };

        }
    }
}
