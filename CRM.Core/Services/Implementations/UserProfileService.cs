﻿using CRM.Core.Dtos;
using CRM.Core.Helpers;
using CRM.Core.Models;
using CRM.Core.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly IMailingService _mailingService;
        public UserProfileService(IUnitOfWork unitOfWork, IAuthService authService, IMailingService mailingService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _mailingService = mailingService;
        }
        public async Task<ResultDto> UpdateNameAsync(string email, UpdateNameDto dto)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["User not found"]

                };
            }
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            var result = await _unitOfWork.UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Name updated successfully"
                };
            }
            return new ResultDto
            {
                IsSuccess = false,
                Errors = ["Something wrong, name was not updated"]

            };
        }
        public async Task<ResultDto> UpdateUsername(string email, string username)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["User not found"]
                };
            }
            if(user.UserName == username)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Username is the same as the current username"]
                };
            }
            if(_unitOfWork.UserManager.Users.Any(u => u.UserName == username))
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Username already used"]
                };
            }
            user.UserName = username;
            var result = await _unitOfWork.UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Username updated successfully"
                };
            }
            return new ResultDto
            {
                IsSuccess = false,
                Errors = ["Something wrong, username was not updated"]
            };
        }
        public async Task<ResultDto> UpdatePasswordAsync(string email, UpdatePasswordDto dto)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["User not found"]
                };
            }
            if (!await _unitOfWork.UserManager.CheckPasswordAsync(user, dto.CurrentPassword))
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Current password is not correct"]
                };
            }
            var result = await _unitOfWork.UserManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (result.Succeeded)
            {
                await _authService.RevokeToken(user.RefreshTokens.FirstOrDefault(r => r.IsActive).Token);
                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Password updated successfully"
                };
            }
            return new ResultDto
            {
                IsSuccess = false,
                Errors = ["Something wrong, password was not updated"]
            };
        }
        public async Task<ResultDto> UpdateEmailAsync(string email, string Newemail)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors= ["User not found"]
                };
            }
            if (user.Email == Newemail)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Email is the same as the current email"]
                };
            }
            if (_unitOfWork.UserManager.Users.Any(u => u.Email == Newemail))
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Email already exists"]
                };
            }


            Random rnd = new Random();
            var randomNum = (rnd.Next(100000, 999999)).ToString();
            string message = "Hi " + user.UserName + " Your new email confirmation code is: " + randomNum;
            try
            {
                var emailResult = await _mailingService.SendEmailAsync(Newemail, "Email Confirmation Code ", message, null);
                if (emailResult)
                {
                    var Vcode = new VerificationCode
                    {
                        Code = randomNum,
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    };
                    await _unitOfWork.VerificationCodes.AddAsync(Vcode);
                    _unitOfWork.complete();
                    return new ResultDto
                    {
                        IsSuccess = true,
                        Message = "Email confirmation was sent to the email successfully !!",
                    };
                }
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Something wrong, confirmation email failed to send"]
                };
            }
            catch
            {
                return new ResultDto { Errors =["Something wrong, confirmation email failed to send"] };
            }
        }
        public async Task<ResultDto> DeleteMyAccount(string email, string password)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["User not found"]
                };
            }
            var result = await _unitOfWork.UserManager.CheckPasswordAsync(user, password);
            if (!result)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Password is incorrect"]
                };
            }
            if (await _unitOfWork.UserManager.IsInRoleAsync(user, "Sales Representative"))
            {
                var customers = await _unitOfWork.Customers.GetAllAsync(c => c.SalesRepresntative == user, ["SalesRepresntative"]);
                foreach (var customer in customers)
                {
                    customer.SalesRepresntative = null;
                }

            }
            var result2 = await _unitOfWork.UserManager.DeleteAsync(user);
            if (result2.Succeeded)
            {
                // invalidate the user token if the account was deleted
                if (user.RefreshTokens.Any(r => r.IsActive))
                {
                    await _authService.RevokeToken(user.RefreshTokens.FirstOrDefault(r => r.IsActive).Token);
                }
                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Account deleted successfully"
                };
            }
            return new ResultDto
            {
                IsSuccess = false,
                Errors = ["Something wrong, account was not deleted"]
            };
        }
    }
}
