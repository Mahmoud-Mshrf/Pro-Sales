using CRM.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Interfaces
{
    public interface IManagerService
    {
        Task<ResultDto> AddSource(string name);
        Task<ResultDto> AddInterest(string name);
        Task<List<UserViewModel>> GetAllUsers();
        List<string> GetAllRoles();
        Task<UserRolesDTO> ViewUserRoles(string userId);
    }
}
