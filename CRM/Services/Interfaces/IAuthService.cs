using CRM.Dtos;
using CRM.Models;

namespace CRM.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModel> GetTokenAsync(TokenRequestDto dto);
    }
}
