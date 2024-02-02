﻿using CRM.Dtos;
using CRM.Models;

namespace CRM.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModel> GetTokenAsync(TokenRequestDto dto);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeToken(string token);
        Task<ResultDto> RegisterAsync(RegisterDto dto); 
    }
}
