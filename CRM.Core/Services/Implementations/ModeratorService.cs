﻿using CRM.Core.Dtos;
using CRM.Core.Models;
using CRM.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Implementations
{
    public class ModeratorService:IModeratorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ModeratorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
         // Will be used after adding Manager module
        public async Task<ReturnUsersDto> GetAllSalesRepresentatives()
        {
            var salesReps = await _unitOfWork.UserManager.GetUsersInRoleAsync("SalesRepresentative");
            if (salesReps == null)
            {
                return new ReturnUsersDto
                {
                    IsSuccess = false,
                    Message = "No sales representatives found",
                };
            }
            var Representatives = new List<UserDto>();
            foreach (var rep in salesReps)
            {
                var user = new UserDto
                {
                    Name = rep.FirstName + " " + rep.LastName,
                    Email = rep.Email,
                    UserId = rep.Id
                };
                Representatives.Add(user);
            }
            return new ReturnUsersDto
            {
                IsSuccess = true,
                Message = "Sales representatives found",
                Users = Representatives
            };
        }

        public async Task<ResultDto> AddCustomer(CustomerDto customerDto,string marketingModeratorEmail)
        {
            var customer = new Customer();
            var salesRep = await _unitOfWork.UserManager.FindByIdAsync(customerDto.SalesRepresntativeId);
            if(salesRep == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Sales Representative not found"
                };
            }
            var MarketingModerator = await _unitOfWork.UserManager.FindByEmailAsync(marketingModeratorEmail);
            if(MarketingModerator == null)
            {
                return new ResultDto 
                {
                    IsSuccess = false,
                    Message = "Marketing Moderator not found"
                };
            }
            var source = await _unitOfWork.Sources.GetByIdAsync(customerDto.sourceId);
            if(source == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Source not found"
                };
            }
            foreach (var interestt in customerDto.UserInterests)
            {
                var interest = await _unitOfWork.Interests.GetByIdAsync(interestt.Id);
                if (interest == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Message = "Interest not found"
                    };
                }
                customer.Interests.Add(interest);
            }
            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.Email = customerDto.Email;
            customer.Phone = customerDto.Phone;
            customer.City = customerDto.City;
            customer.Age = customerDto.Age;
            customer.Gender = customerDto.Gender;
            customer.AdditionDate = DateTime.Now;
            customer.SalesRepresntative = salesRep;
            customer.Source = source;
            customer.MarketingModerator = MarketingModerator;

            await _unitOfWork.Customers.AddAsync(customer);
            try
            {
                _unitOfWork.complete();
            }
            catch (Exception e)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Customer added successfully"
            };
        }
        public async Task<ResultDto> UpdateCustomer(CustomerDto customerDto,int customerId)
        {
            var customer = await _unitOfWork.Customers.FindAsync(c => c.CustomerId == customerId, ["Interests"]);
            if (customer == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Customer not found"
                };
            }
            var salesRep = await _unitOfWork.UserManager.FindByIdAsync(customerDto.SalesRepresntativeId);
            if (salesRep == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Sales Representative not found"
                };
            }
            var source = await _unitOfWork.Sources.GetByIdAsync(customerDto.sourceId);
            if (source == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Source not found"
                };
            }
            //var interest = await _unitOfWork.Interests.GetAllAsync(customerDto.);
            foreach(var interestt in customerDto.UserInterests)
            {
                var interest = await _unitOfWork.Interests.GetByIdAsync(interestt.Id);
                if(interest == null)
                {
                    return new ResultDto
                    {
                        IsSuccess = false,
                        Message = "Interest not found"
                    };
                }
                if (customer.Interests.Any(i => i.InterestID == interest.InterestID) && interestt.IsSelected)
                {
                    continue;
                }
                if (!customer.Interests.Any(i => i.InterestID == interest.InterestID) && !interestt.IsSelected)
                {
                    continue;
                }
                if ((!customer.Interests.Any(i => i.InterestID == interest.InterestID)) && interestt.IsSelected)
                {
                    customer.Interests.Add(interest);
                }
                if (customer.Interests.Any(i => i.InterestID == interest.InterestID) && !interestt.IsSelected)
                {
                    customer.Interests.Remove(interest);
                }


            }
            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.Email = customerDto.Email;
            customer.Phone = customerDto.Phone;
            customer.City = customerDto.City;
            customer.Age = customerDto.Age;
            customer.SalesRepresntative = salesRep;
            customer.Source = source;
            try
            {
                _unitOfWork.complete();
            }
            catch (Exception e)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
            return new ResultDto
            {
                IsSuccess = true,
                Message = "Customer updated successfully"
            };
        }
        
    }
}
