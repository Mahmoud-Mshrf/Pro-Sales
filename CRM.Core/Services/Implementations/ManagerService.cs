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

        public async Task<ResultDto> AddInterest(string name)
        {

            var interest = new Interest
            {
                InterestName = name
            };
            var interests = await _unitOfWork.Interests.GetAllAsync();
            if (interests.Any(interests => interests.InterestName.ToLower() == name.ToLower()))
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Interest already exists"]
                };
            }
            var result = await _unitOfWork.Interests.AddAsync(interest);
            _unitOfWork.complete();
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Interest added successfully"
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
                Roles = _unitOfWork.UserManager.GetRolesAsync(user).Result
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
        public async Task<ReturnUserRolesDto> ManageUserRoles(UserRolesDTO dto)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(dto.Id);
            var returnUserRolesdto = new ReturnUserRolesDto();

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
            var UserRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            foreach (var role in dto.Roles)
            {
                if (UserRoles.Any(r => r == role.Name) && !role.IsSelected)
                    await _unitOfWork.UserManager.RemoveFromRoleAsync(user, role.Name);
                if (!UserRoles.Any(r => r == role.Name) && role.IsSelected)
                    await _unitOfWork.UserManager.AddToRoleAsync(user, role.Name);
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
            if (user is not null && !roles.IsNullOrEmpty())
            {
                await _unitOfWork.UserManager.DeleteAsync(user);
            }
        }
        public async Task<ResultDto> AddBusinessInfo(string email,BusinessDto dto)
        {
            var Manager = await _unitOfWork.UserManager.FindByEmailAsync(email);
            var businesses = await _unitOfWork.Businesses.GetAllAsync();
            var existingBusiness = businesses.FirstOrDefault();
            if (existingBusiness != null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Business information is already exist , go to update it"]
                };
                
            }
            var business = new Business
            {
                CompanyName = dto.CompanyName,
                Description = dto.Description,
                Manager = Manager
            };
            var result = await _unitOfWork.Businesses.AddAsync(business);
            try
            {
                _unitOfWork.complete();
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [ex.Message]
                };
            }

            return new ResultDto
            {
                IsSuccess = true,
                Message = "Business information added successfully"
            };
        }
        public async Task<ResultDto> UpdateBusinessInfo(string email,BusinessDto dto)
        {
            var Manager = await _unitOfWork.UserManager.FindByEmailAsync(email);
            var businesses = await _unitOfWork.Businesses.GetAllAsync();
            var existingBusiness = businesses.FirstOrDefault();
            if (existingBusiness == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Business information is not exist , go to add it"]
                };
            }
            existingBusiness.CompanyName = dto.CompanyName;
            existingBusiness.Description = dto.Description;
            existingBusiness.Manager = Manager; 
            var result = _unitOfWork.Businesses.Update(existingBusiness);
            try
            {
                _unitOfWork.complete();
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [ex.Message]
                };
            }

            return new ResultDto
            {
                IsSuccess = true,
                Message = "Business information updated successfully"
            };
        }
        public async Task<BusinessDto> GetBussinesInfo()
        {
            var businesses = await _unitOfWork.Businesses.GetAllAsync();
            var existingBusiness = businesses.FirstOrDefault();
            if (existingBusiness == null)
            {
                return new BusinessDto();
            }
            var businessDto = new BusinessDto
            {
                CompanyName = existingBusiness.CompanyName,
                Description = existingBusiness.Description
            };
            return businessDto;
        }
    }
}
