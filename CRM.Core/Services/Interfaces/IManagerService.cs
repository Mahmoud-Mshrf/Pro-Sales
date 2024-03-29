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
        Task<ResultDto> AddInterest(string name);
        Task<InterestDto> updateInterest(InterestDto dto);
        Task<InterestDto> getInterest(int id);
        Task<ResultDto> DeleteInterest(int id);
        Task<SourceDto> updateSource(SourceDto dto);
        Task<SourceDto> getSource(int id);
        Task<ResultDto> DeleteSource(int id);
        Task<List<UserViewModel>> GetAllUsers();
        List<string> GetAllRoles();
        Task<ReturnUserRolesDto> ViewUserRoles(string userId);
        Task<ReturnUserRolesDto> ManageUserRoles(UserRolesDTO dto);
        //Task<BusinessDto> GetBussinesInfo();
        Task<ResultDto> UpdateBusinessInfo(string email,BusinessDto dto);
        Task<ResultDto> AddBusinessInfo(string email, BusinessDto dto);
    }
}
