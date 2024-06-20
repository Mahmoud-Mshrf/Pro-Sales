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
        Task<InterestDto> AddInterest(string name);
        Task<ReturnInterestDto> updateInterest(InterestDto dto);
        Task<InterestDto> getInterest(int id);
        //Task<ReturnInterestDto> DisableInterest(int id);
        Task<SourceDto> updateSource(SourceDto dto);
        Task<SourceDto> getSource(int id);
        Task<ResultDto> DeleteSource(int id);
        Task<List<UserViewModel>> GetAllUsers();
        List<string> GetAllRoles();
        Task<ReturnUserRolesDto> ViewUserRoles(string userId);
        Task<ReturnUserRolesDto> ManageUserRoles(UserRolesDTO dto,string ManagerId);
        //Task<BusinessDto> GetBussinesInfo();
        Task<BusinessDto> UpdateBusinessInfo(string email, BusinessDto dto);
    }
}
