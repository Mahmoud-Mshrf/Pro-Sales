using CRM.Dtos;
using CRM.Helpers;
using CRM.Models;
using CRM.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
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

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
        }

        public Task<AuthModel> GetTokenAsync(TokenRequestDto dto)
        {
            throw new NotImplementedException();
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

    }
}
