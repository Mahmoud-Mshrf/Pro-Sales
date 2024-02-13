using CRM.Core.Dtos;
using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<ResultDto> UpdateNameAsync(string email, UpdateNameDto dto);
        Task<ResultDto> UpdateEmailAsync(string userName, string newEmail);
        Task<ResultDto> UpdatePasswordAsync(string email, UpdatePasswordDto dto);


    }
}
