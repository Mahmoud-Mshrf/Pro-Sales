using CRM.Core.Dtos;
using CRM.Core.Models;
using CRM.Core.Services.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Implementations
{
    public class ManagerService:IManagerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ManagerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<InterestDto> AddInterest(string name)
        {
            var interest = new Interest
            {
                InterestName = name
            };
            var interests = await _unitOfWork.Interests.GetAllAsync();
            if (interests.Any(interests => interests.InterestName.ToLower() == name.ToLower()))
            {
                return new InterestDto();
            }
            var result = await _unitOfWork.Interests.AddAsync(interest);
            _unitOfWork.complete();
            return new InterestDto
            {
                Id= result.InterestID,
                Name= result.InterestName,
                IsDisabled= result.IsDisabled
            };
        }
        public async Task<ReturnInterestDto> updateInterest(InterestDto dto)
        {
            var interest = await _unitOfWork.Interests.GetByIdAsync(dto.Id);
            if (interest == null)
            {
                return new ReturnInterestDto
                {
                    IsSuccess = false,
                    Errors = ["Interest not found"]
                };
            }
            // check if there is another interest with the new name
            var interests = await _unitOfWork.Interests.GetAllAsync();
            var sameNameInterest = interests.FirstOrDefault(interest => interest.InterestName.ToLower() == dto.Name.ToLower());
            if(sameNameInterest!=null && sameNameInterest!=interest)
            {
                return new ReturnInterestDto
                {
                    IsSuccess = false,
                    Errors = ["Interest already exists"]
                };
            }
            interest.InterestName = dto.Name;
            interest.IsDisabled = dto.IsDisabled;
            _unitOfWork.Interests.Update(interest);
            _unitOfWork.complete();
            return new ReturnInterestDto
            {
                IsSuccess = true,
                Id = interest.InterestID,
                Name = interest.InterestName,
                IsDisabled = interest.IsDisabled
            };
        }
        public async Task<InterestDto> getInterest(int id)
        {
            var interest = await _unitOfWork.Interests.GetByIdAsync(id);
            if (interest == null)
            {
                return new InterestDto();
            }
            return new InterestDto
            {
                Id = interest.InterestID,
                Name = interest.InterestName,
                IsDisabled = interest.IsDisabled
            };
        }
        //public async Task<ReturnInterestDto> DisableInterest(int id)
        //{
        //    var interest = await _unitOfWork.Interests.GetByIdAsync(id);
        //    if (interest == null)
        //    {
        //        return new ReturnInterestDto
        //        {
        //            IsSuccess = false
        //        };
        //    }
        //    //_unitOfWork.Interests.Delete(interest);
        //    interest.IsDisabled = true;
        //    _unitOfWork.complete();
        //    return new ReturnInterestDto
        //    {
        //        IsSuccess=true,
        //        Id = interest.InterestID,
        //        Name = interest.InterestName,
        //        IsDisabled = interest.IsDisabled
        //    };
        //}
        public async Task<SourceDto> updateSource(SourceDto dto)
        {
            var source = await _unitOfWork.Sources.GetByIdAsync(dto.Id);
            if (source == null)
            {
                return new SourceDto();
            }
            source.SourceName = dto.Name;
            _unitOfWork.Sources.Update(source);
            _unitOfWork.complete();
            return new SourceDto
            {
                Id = source.SourceId,
                Name = source.SourceName
            };
        }
        public async Task<SourceDto> getSource(int id)
        {
            var source = await _unitOfWork.Sources.GetByIdAsync(id);
            if (source == null)
            {
                return new SourceDto();
            }
            return new SourceDto
            {
                Id = source.SourceId,
                Name = source.SourceName
            };
        }
        public async Task<ResultDto> DeleteSource(int id)
        {
            var source = await _unitOfWork.Sources.GetByIdAsync(id);
            if (source == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Source not found"]
                };
            }
            _unitOfWork.Sources.Delete(source);
            _unitOfWork.complete();
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Source deleted successfully"
            };
        }
        public async Task<List<UserViewModel>> GetAllUsers()
        {
            var users = await _unitOfWork.UserManager.Users.ToListAsync();

            var userViewModels = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Email = user.Email,
                Roles = _unitOfWork.UserManager.GetRolesAsync(user).Result,
                EmailConfirmed = user.EmailConfirmed
            }).ToList();

            return userViewModels;
        }
        public List<string> GetAllRoles()
        {
            var roles = _unitOfWork.RoleManager.Roles.Select(x => x.Name).ToList();
            return roles;
        }
        public async Task<ReturnUserRolesDto> ViewUserRoles(string userId)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(userId);
            var returnUserRolesdto = new ReturnUserRolesDto();
            if (user is null)
            {
                returnUserRolesdto.IsSucces = false;
                returnUserRolesdto.Errors = ["User not found"];
                return returnUserRolesdto;
            }
            var roles = await _unitOfWork.RoleManager.Roles.ToListAsync();
            returnUserRolesdto.IsSucces = true;
            returnUserRolesdto.Errors = null;
            returnUserRolesdto.Id = user.Id;
            //returnUserRolesdto.Username = user.UserName;
            returnUserRolesdto.Roles = roles.Select(r => new RoleModel
            {
                //Id = r.Id,
                Name = r.Name,
                IsSelected = _unitOfWork.UserManager.IsInRoleAsync(user, r.Name).Result
            });
            return returnUserRolesdto;
        }
        public async Task<ReturnUserRolesDto> ManageUserRoles(UserRolesDTO dto,string ManagerId)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(dto.Id);
            var returnUserRolesdto = new ReturnUserRolesDto();
            var manager= await _unitOfWork.UserManager.FindByIdAsync(ManagerId);
            if (user is null)
            {
                returnUserRolesdto.IsSucces = false;
                returnUserRolesdto.Errors = ["User not found"];
                return returnUserRolesdto;
            }
            foreach(var role in dto.Roles)
            {
                if(!await _unitOfWork.RoleManager.RoleExistsAsync(role.Name))
                {
                    returnUserRolesdto.IsSucces = false;
                    returnUserRolesdto.Errors = [$"{role.Name} role not found"];
                    return returnUserRolesdto;
                }
            }
            var userClaims= await _unitOfWork.UserManager.GetClaimsAsync(user);
            if (userClaims.Any(c => c.Type == "SuperAdmin")&& user !=manager)
            {
                returnUserRolesdto.IsSucces = false;
                returnUserRolesdto.Errors = ["You can't change the role of the super admin"];
                return returnUserRolesdto;
            }
            var WasSalesRep = await _unitOfWork.UserManager.IsInRoleAsync(user, "Sales Representative");
            var WasModerator = await _unitOfWork.UserManager.IsInRoleAsync(user, "Marketing Moderator");
            var WasManager = await _unitOfWork.UserManager.IsInRoleAsync(user, "Manager");
            var UserRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            foreach (var role in dto.Roles)
            {
                if (UserRoles.Any(r => r == role.Name) && !role.IsSelected)
                    await _unitOfWork.UserManager.RemoveFromRoleAsync(user, role.Name);
                if (!UserRoles.Any(r => r == role.Name) && role.IsSelected)
                    await _unitOfWork.UserManager.AddToRoleAsync(user, role.Name);
            }
            var IsSalesRep = await _unitOfWork.UserManager.IsInRoleAsync(user, "Sales Representative");
            var IsModerator = await _unitOfWork.UserManager.IsInRoleAsync(user, "Marketing Moderator");

            if ((WasSalesRep && !IsSalesRep) && (!IsModerator))
            {
                var customers = await _unitOfWork.Customers.GetAllAsync(x => x.SalesRepresntative.Id == user.Id);
                foreach (var customer in customers)
                {
                    customer.SalesRepresntative = null;
                    _unitOfWork.Customers.Update(customer);
                    _unitOfWork.complete();
                }
            }
            var IsManager = await _unitOfWork.UserManager.IsInRoleAsync(user, "Manager");
            if (!WasManager && IsManager)
            {
                var customers = await _unitOfWork.Customers.GetAllAsync(x => x.SalesRepresntative.Id == user.Id);
                foreach (var customer in customers)
                {
                    customer.SalesRepresntative = null;
                    _unitOfWork.Customers.Update(customer);
                    _unitOfWork.complete();
                }
            }
            if (WasModerator && !IsModerator)
            {
                var customers = await _unitOfWork.Customers.GetAllAsync(x => x.SalesRepresntative.Id == user.Id);
                foreach (var customer in customers)
                {
                    customer.SalesRepresntative = null;
                    _unitOfWork.Customers.Update(customer);
                    _unitOfWork.complete();
                }
            }
            BackgroundJob.Schedule(() => DeletUserWithoutRoles(dto.Id), TimeSpan.FromDays(5));
            var roles = await _unitOfWork.RoleManager.Roles.ToListAsync();
            var UserRolesDto = new ReturnUserRolesDto
            {
                IsSucces = true,
                Id = dto.Id,
                //Username = dto.Username,
                Errors = null,
                Roles = roles.Select(r => new RoleModel
                {
                    //Id = r.Id,
                    Name = r.Name,
                    IsSelected = _unitOfWork.UserManager.IsInRoleAsync(user, r.Name).Result
                })
            };
            return UserRolesDto;
        }
        public async Task DeletUserWithoutRoles(string id)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            var roles = await _unitOfWork.UserManager.GetRolesAsync(user);
            if (user!=null && roles.IsNullOrEmpty())
            {
                await _unitOfWork.UserManager.DeleteAsync(user);
            }
        }
        public async Task<BusinessDto> UpdateBusinessInfo(string email,BusinessDto dto)
        {
            var Manager = await _unitOfWork.UserManager.FindByEmailAsync(email);
            var businesses = await _unitOfWork.Businesses.GetAllAsync();
            var existingBusiness = businesses.FirstOrDefault();
            if (existingBusiness != null)
            {
                existingBusiness.CompanyName = dto.CompanyName;
                existingBusiness.Description = dto.Description;
                existingBusiness.Manager = Manager;
                _unitOfWork.Businesses.Update(existingBusiness);
                _unitOfWork.complete();
                return new BusinessDto
                {
                    CompanyName = existingBusiness.CompanyName,
                    Description = existingBusiness.Description,
                };
            }
            var business = new Business
            {
                CompanyName = dto.CompanyName,
                Description = dto.Description,
                Manager = Manager
            };
            var result = await _unitOfWork.Businesses.AddAsync(business);
            _unitOfWork.complete();
            return new BusinessDto
            {
                CompanyName= business.CompanyName,
                Description= business.Description,
            };

            //try
            //{
            //    _unitOfWork.complete();
            //}
            //catch (Exception ex)
            //{
            //    return new ResultDto
            //    {
            //        IsSuccess = false,
            //        Errors = [ex.Message]
            //    };
            //}
        }
    }
}
