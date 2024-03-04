using CRM.Core.Dtos;
using CRM.Core.Models;
using CRM.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ResultDto> AddSource(string name)
        {

            var interest = new Source
            {
                SourceName = name
            };
            var sources = await _unitOfWork.Sources.GetAllAsync();
            if (sources.Any(sources => sources.SourceName == name))
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Source already exists"]
                };
            }
            var result = await _unitOfWork.Sources.AddAsync(interest);
            _unitOfWork.complete();
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Source added successfully"
            };
        }
        public async Task<ResultDto> AddInterest(string name)
        {

            var interest = new Interest
            {
                InterestName = name
            };
            var interests = await _unitOfWork.Interests.GetAllAsync();
            if (interests.Any(interests => interests.InterestName == name))
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
                UserName = user.UserName,
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
        public async Task<UserRolesDTO> ViewUserRoles(string userId)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(userId);
            var userRolesdto = new UserRolesDTO();
            if (user is null)
            {
                userRolesdto.ErrorMessage = "No user with this id";
                return userRolesdto;
            }
            var roles = await _unitOfWork.RoleManager.Roles.ToListAsync();

            userRolesdto.ErrorMessage = string.Empty;
            userRolesdto.Id = user.Id;
            userRolesdto.UserName = user.UserName;
            userRolesdto.Roles = roles.Select(r => new RoleModel
            {
                Id = r.Id,
                Name = r.Name,
                IsSelected = _unitOfWork.UserManager.IsInRoleAsync(user, r.Name).Result
            });
            return userRolesdto;
        }
        public async Task<UserRolesDTO> ManageUserRoles(UserRolesDTO dto)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(dto.Id);
            if (user is null)
            {
                dto.ErrorMessage = "No User With This Id";
                return dto;
            }

            var UserRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            foreach (var role in dto.Roles)
            {
                if (UserRoles.Any(r => r == role.Name) && !role.IsSelected)
                    await _unitOfWork.UserManager.RemoveFromRoleAsync(user, role.Name);
                if (!UserRoles.Any(r => r == role.Name) && role.IsSelected)
                    await _unitOfWork.UserManager.AddToRoleAsync(user, role.Name);
            }

            var roles = await _unitOfWork.RoleManager.Roles.ToListAsync();

            var UserRolesDto = new UserRolesDTO
            {
                Id = dto.Id,
                UserName = dto.UserName,
                ErrorMessage = string.Empty,
                Roles = roles.Select(r => new RoleModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsSelected = _unitOfWork.UserManager.IsInRoleAsync(user, r.Name).Result
                })
            };
            return UserRolesDto;
        }
    }
}
