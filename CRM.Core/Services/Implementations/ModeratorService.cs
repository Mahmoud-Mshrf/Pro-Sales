using CRM.Core.Dtos;
using CRM.Core.Models;
using CRM.Core.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Implementations
{
    public class ModeratorService : IModeratorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFilterService _filterService;
        public ModeratorService(IUnitOfWork unitOfWork, IFilterService filterService)
        {
            _unitOfWork = unitOfWork;
            _filterService = filterService;
        }
        // Will be used after adding Manager module
        public async Task<ReturnUsersDto> GetAllSalesRepresentatives()
        {
            var salesReps = await _unitOfWork.UserManager.GetUsersInRoleAsync("Sales Representative");
            if (salesReps == null)
            {
                return new ReturnUsersDto
                {
                    IsSuccess = false,
                    Errors = ["No sales representatives found"]
                };
            }
            var Representatives = new List<UserDto>();
            foreach (var rep in salesReps)
            {
                var customersCount = await _unitOfWork.Customers.CountAsync(c => c.SalesRepresntative.Id == rep.Id);
                var user = new UserDto
                {
                    FirstName= rep.FirstName,
                    LastName=rep.LastName,
                    Email = rep.Email,
                    UserName=rep.UserName,
                    Roles=await _unitOfWork.UserManager.GetRolesAsync(rep),
                    Id = rep.Id,
                    customers = customersCount
                };
                Representatives.Add(user);
            }
            return new ReturnUsersDto
            {
                IsSuccess = true,
                Users = Representatives
            };
        }
        public async Task<ResultDto> DeleteCustomer(int customerId)
        {
            var customer = await _unitOfWork.Customers.FindAsync(c=>c.CustomerId==customerId && !c.IsDeleted);
            if (customer == null)
                return new ResultDto { IsSuccess = false, Errors = ["Customer not found"] };

            //_unitOfWork.Customers.Delete(customerId);\
            customer.IsDeleted = true;
            try
            {
                _unitOfWork.complete();
            }
            catch (Exception e)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = [e.Message]
                };
            }

            return new ResultDto { IsSuccess = true, Message = "Customer deleted successfully" };

        }
        public async Task<ReturnCustomerDto> GetCustomer(int customerId)
        {
            var customerDto = new ReturnCustomerDto();
            var customer = await _unitOfWork.Customers.FindAsync(c => c.CustomerId == customerId && !c.IsDeleted, ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            if (customer == null)
            {
                customerDto.IsSuccess = false;
                customerDto.Errors = ["Customer not found"];
                return customerDto;
            }
            customerDto.IsSuccess = true;
            customerDto.Id = customer.CustomerId;
            customerDto.FirstName = customer.FirstName;
            customerDto.LastName = customer.LastName;
            customerDto.Email = customer.Email;
            customerDto.Phone = customer.Phone;
            customerDto.City = customer.City;
            customerDto.Age = customer.Age;
            customerDto.Gender = customer.Gender;
            //customerDto.SalesRepresntativeId = customer.SalesRepresntative.Id;
            //customerDto.SourceId = customer.Source.SourceId;
            customerDto.Source = new SourceDto
            {
                Id = customer.Source.SourceId,
                Name = customer.Source.SourceName
            };
            customerDto.AdditionDate = customer.AdditionDate;
            var interests = await _unitOfWork.Interests.GetAllAsync();
            if (interests == null)
            {
                customerDto.IsSuccess = false;
                customerDto.Errors = ["No interests found"];
                return customerDto;
            }
            customerDto.Interests = new List<UserInterestDto>();
            //foreach(var interest in interests)
            //{
            //    if(customer.Interests.Any(i=>i.InterestID==interest.InterestID))
            //    {
            //        customerDto.Interests.Add(new UserInterestDto { /*Id = interest.InterestID,*/ Name = interest.InterestName, IsSelected = true });
            //    }
            //    else
            //    {
            //        customerDto.Interests.Add(new UserInterestDto { /*Id = interest.InterestID,*/ Name = interest.InterestName, IsSelected = false });
            //    }
            //}
            //foreach (var interest in customer.Interests)
            //{
            //    customerDto.Interests.Add(new UserInterestDto { Id = interest.InterestID, Name = interest.InterestName });
            //}
            customerDto.Interests = customer.Interests.Select(i => new UserInterestDto { Id = i.InterestID, Name = i.InterestName }).ToList();
            var lastAction = await GetLastAction(customer.CustomerId);
            if (lastAction.Summary != null)
            {
                customerDto.LastAction = lastAction;
            }
            var userdto = new UserDto
            {
                Id = customer.SalesRepresntative.Id,
                FirstName=customer.SalesRepresntative.FirstName,
                LastName=customer.SalesRepresntative.LastName,
                UserName = customer.SalesRepresntative.UserName,
                Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.SalesRepresntative),
                Email = customer.SalesRepresntative.Email,
                customers = await _unitOfWork.Customers.CountAsync(c => c.SalesRepresntative.Id == customer.SalesRepresntative.Id)
            };
            customerDto.SalesRepresentative = userdto;
            var userdto2 = new UserDto
            {
                Id = customer.MarketingModerator.Id,
                FirstName = customer.MarketingModerator.FirstName,
                LastName = customer.MarketingModerator.LastName,
                UserName = customer.MarketingModerator.UserName,
                Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.MarketingModerator),
                Email = customer.MarketingModerator.Email
            };
            customerDto.AddedBy = userdto2;
            //customerDto.UserInterests = customer.Interests.Select(i => new UserInterestDto { /*Id = i.InterestID,*/ Name = i.InterestName }).ToList();
            return customerDto;
        }
        public async Task<ReturnAllCustomersDto> GetAllCustomers(int page, int size)
        {
            var customers = await _unitOfWork.Customers.GetAllAsync(c=>!c.IsDeleted,["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            if (customers == null)
            {
                return new ReturnAllCustomersDto
                {
                    IsSuccess = false,
                    Errors = ["No customers found"]
                };
            }
            var customersDto = new List<ReturnCustomerDto>();
            foreach (var customer in customers)
            {
                var customerDto = new ReturnCustomerDto
                {
                    //CustomerId = customer.CustomerId,
                    Id = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    City = customer.City,
                    Age = customer.Age,
                    Gender = customer.Gender,
                    //SalesRepresntativeId = customer.SalesRepresntative.Id,
                    //SourceId = customer.Source.SourceId,
                    //Interests = customer.Interests.Select(i => new UserInterestDto { /*Id = i.InterestID,*/ Name = i.InterestName, IsSelected = true }).ToList(),
                    AdditionDate = customer.AdditionDate
                };
                customerDto.Source = new SourceDto
                {
                    Id = customer.Source.SourceId,
                    Name = customer.Source.SourceName
                };
                //foreach (var interest in customer.Interests)
                //{
                //    customerDto.Interests.Add(new UserInterestDto { Id = interest.InterestID, Name = interest.InterestName });
                //}
                customerDto.Interests = customer.Interests.Select(i => new UserInterestDto { Id = i.InterestID, Name = i.InterestName }).ToList();
                var lastAction = await GetLastAction(customer.CustomerId);
                if (lastAction.Summary != null)
                {
                    customerDto.LastAction = lastAction;
                }
                var userdto = new UserDto
                {
                    Id = customer.SalesRepresntative.Id,
                    FirstName = customer.SalesRepresntative.FirstName,
                    LastName = customer.SalesRepresntative.LastName,
                    UserName = customer.SalesRepresntative.UserName,
                    Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.SalesRepresntative),
                    Email = customer.SalesRepresntative.Email,
                    customers = await _unitOfWork.Customers.CountAsync(c => c.SalesRepresntative.Id == customer.SalesRepresntative.Id)
                };
                customerDto.SalesRepresentative = userdto;
                var userdto2 = new UserDto
                {
                    Id = customer.MarketingModerator.Id,
                    FirstName = customer.MarketingModerator.FirstName,
                    LastName = customer.MarketingModerator.LastName,
                    UserName = customer.MarketingModerator.UserName,
                    Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.MarketingModerator),
                    Email = customer.MarketingModerator.Email
                };
                customerDto.AddedBy = userdto2;
                customersDto.Add(customerDto);
            }
            var Customers = customersDto.OrderByDescending(DateTime => DateTime.AdditionDate).ToList();
            var CustomerPage = _filterService.Paginate(Customers, page, size);
            return new ReturnAllCustomersDto
            {
                IsSuccess = true,
                Pages = CustomerPage
                //Customers = customersDto.OrderByDescending(DateTime => DateTime.AdditionDate).ToList()
            };

        }
        public async Task<ReturnCustomerDto> AddCustomer(AddCustomerDto customerDto, string marketingModeratorEmail)
        {
            var customer = new Customer();
            var salesRep = await _unitOfWork.UserManager.FindByIdAsync(customerDto.SalesRepresentativeId);
            if (salesRep == null)
            {
                return new ReturnCustomerDto
                {
                    IsSuccess = false,
                    Errors = ["Sales Representative not found"]
                };
            }
            var MarketingModerator = await _unitOfWork.UserManager.FindByEmailAsync(marketingModeratorEmail);
            if (MarketingModerator == null)
            {
                return new ReturnCustomerDto
                {
                    IsSuccess = false,
                    Errors = ["Marketing Moderator not found"]
                };
            }
            //var source = await _unitOfWork.Sources.FindAsync(x=>x.SourceName==customerDto.sourceName);
            var source = await _unitOfWork.Sources.FindAsync(x => x.SourceId == customerDto.SourceId);

            if (source == null)
            {
                return new ReturnCustomerDto
                {
                    IsSuccess = false,
                    Errors = ["Source not found"]
                };
            }
            foreach (var interestt in customerDto.Interests)
            {
                var interest = await _unitOfWork.Interests.FindAsync(x => x.InterestID == interestt.Id);
                if (interest == null)
                {
                    return new ReturnCustomerDto
                    {
                        IsSuccess = false,
                        Errors = [$"{interest}  Interest not found"]
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
                return new ReturnCustomerDto
                {
                    IsSuccess = false,
                    Errors = [e.Message]
                };
            }
            var ReturnCustomerDto = new ReturnCustomerDto
            {
                AdditionDate = customer.AdditionDate,
                Age = customer.Age,
                Gender = customer.Gender,
                City = customer.City,
                //CustomerId=customer.CustomerId,
                Email = customer.Email,
                Phone = customer.Phone,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                //SourceId = customer.Source.SourceId,
                IsSuccess = true,
                Id = customer.CustomerId,

            };
            ReturnCustomerDto.Source = new SourceDto
            {
                Id = customer.Source.SourceId,
                Name = customer.Source.SourceName
            };
            var userdto = new UserDto
            {
                Id = customer.SalesRepresntative.Id,
                FirstName = customer.SalesRepresntative.FirstName,
                LastName = customer.SalesRepresntative.LastName,
                UserName = customer.SalesRepresntative.UserName,
                Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.SalesRepresntative),
                Email = customer.SalesRepresntative.Email,
                customers = await _unitOfWork.Customers.CountAsync(c => c.SalesRepresntative.Id == customer.SalesRepresntative.Id)
            };
            ReturnCustomerDto.SalesRepresentative = userdto;
            var userdto2 = new UserDto
            {
                Id = customer.MarketingModerator.Id,
                FirstName = customer.MarketingModerator.FirstName,
                LastName = customer.MarketingModerator.LastName,
                UserName = customer.MarketingModerator.UserName,
                Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.MarketingModerator),
                Email = customer.MarketingModerator.Email
            };
            ReturnCustomerDto.AddedBy = userdto2;
            ReturnCustomerDto.Interests = new List<UserInterestDto>();
            //foreach (var interest in customer.Interests)
            //{
            //    if (customer.Interests.Any(i => i.InterestID == interest.InterestID))
            //    {
            //        ReturnCustomerDto.Interests.Add(new UserInterestDto { /*Id = interest.InterestID,*/ Name = interest.InterestName, IsSelected = true });
            //    }
            //    else
            //    {
            //        ReturnCustomerDto.Interests.Add(new UserInterestDto { /*Id = interest.InterestID,*/ Name = interest.InterestName, IsSelected = false });
            //    }
            //}
            //foreach(var interest in customer.Interests)
            //{
            //    ReturnCustomerDto.Interests.Add(new UserInterestDto { Id = interest.InterestID, Name = interest.InterestName });
            //}
            ReturnCustomerDto.Interests = customer.Interests.Select(i => new UserInterestDto { Id = i.InterestID, Name = i.InterestName }).ToList();

            return ReturnCustomerDto;
        }
        public async Task<ReturnCustomerDto> UpdateCustomer(AddCustomerDto customerDto, int customerId)
        {
            var customer = await _unitOfWork.Customers.FindAsync(c => c.CustomerId == customerId && !c.IsDeleted, ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            if (customer == null)
            {
                return new ReturnCustomerDto
                {
                    IsSuccess = false,
                    Errors = ["Customer not found"]
                };
            }
            var salesRep = await _unitOfWork.UserManager.FindByIdAsync(customerDto.SalesRepresentativeId);
            if (salesRep == null)
            {
                return new ReturnCustomerDto
                {
                    IsSuccess = false,
                    Errors = ["Sales Representative not found"]
                };
            }
            //var source = await _unitOfWork.Sources.FindAsync(x => x.SourceName == customerDto.sourceName);
            var source = await _unitOfWork.Sources.FindAsync(x => x.SourceId == customerDto.SourceId);
            if (source == null)
            {
                return new ReturnCustomerDto
                {
                    IsSuccess = false,
                    Errors = ["Source not found"]
                };
            }
            //var interest = await _unitOfWork.Interests.GetAllAsync(customerDto.);
            var BussinesInterests = await _unitOfWork.Interests.GetAllAsync();
            foreach (var interestt in customerDto.Interests)
            {
                //var interest = await _unitOfWork.Interests.FindAsync(x => x.InterestName == interestt.Name);
                var interest = await _unitOfWork.Interests.FindAsync(x => x.InterestID == interestt.Id);
                if (interest == null)
                {
                    return new ReturnCustomerDto
                    {
                        IsSuccess = false,
                        Errors = ["Interest not found"]
                    };

                }
                if (customer.Interests.Any(i => i.InterestID == interest.InterestID))
                {
                    continue;
                }
                if (!customer.Interests.Any(i => i.InterestID == interest.InterestID))
                    customer.Interests.Add(interest);
                //if (customer.Interests.Any(i => i.InterestID == interest.InterestID) && interestt.IsSelected)
                //{
                //    continue;
                //}
                //if (!customer.Interests.Any(i => i.InterestID == interest.InterestID) && !interestt.IsSelected)
                //{
                //    continue;
                //}
                //if ((!customer.Interests.Any(i => i.InterestID == interest.InterestID)) && interestt.IsSelected)
                //{
                //    customer.Interests.Add(interest);
                //}
                //if (customer.Interests.Any(i => i.InterestID == interest.InterestID) && !interestt.IsSelected)
                //{
                //    customer.Interests.Remove(interest);
                //}
            }
            foreach (var bisInterest in BussinesInterests)
            {
                if (customer.Interests.Any(i => i.InterestID == bisInterest.InterestID) && customerDto.Interests.Any(i => i.Id == bisInterest.InterestID)) { continue; }
                if (customer.Interests.Any(i => i.InterestID == bisInterest.InterestID) && !customerDto.Interests.Any(i => i.Id == bisInterest.InterestID))
                {
                    customer.Interests.Remove(bisInterest);
                }

            }

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.Gender = customerDto.Gender;
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
                return new ReturnCustomerDto
                {
                    IsSuccess = false,
                    Errors = [e.Message]
                };

            }
            var ReturnCustomerDto = new ReturnCustomerDto
            {
                AdditionDate = customer.AdditionDate,
                Age = customer.Age,
                Gender = customer.Gender,
                City = customer.City,
                //CustomerId=customer.CustomerId,
                Email = customer.Email,
                Phone = customer.Phone,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                //SourceId = customer.Source.SourceId,
                IsSuccess = true,
                Id = customer.CustomerId,

            };
            ReturnCustomerDto.Source = new SourceDto
            {
                Id = customer.Source.SourceId,
                Name = customer.Source.SourceName
            };
            var userdto = new UserDto
            {
                Id = customer.SalesRepresntative.Id,
                FirstName = customer.SalesRepresntative.FirstName,
                LastName = customer.SalesRepresntative.LastName,
                UserName = customer.SalesRepresntative.UserName,
                Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.SalesRepresntative),
                Email = customer.SalesRepresntative.Email,
                customers = await _unitOfWork.Customers.CountAsync(c => c.SalesRepresntative.Id == customer.SalesRepresntative.Id)
            };
            ReturnCustomerDto.SalesRepresentative = userdto;
            var userdto2 = new UserDto
            {
                Id = customer.MarketingModerator.Id,
                FirstName = customer.MarketingModerator.FirstName,
                LastName = customer.MarketingModerator.LastName,
                UserName = customer.MarketingModerator.UserName,
                Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.MarketingModerator),
                Email = customer.MarketingModerator.Email
            };
            ReturnCustomerDto.AddedBy = userdto2;
            ReturnCustomerDto.Interests = new List<UserInterestDto>();
            //foreach (var interest in customer.Interests)
            //{
            //    if (customer.Interests.Any(i => i.InterestID == interest.InterestID))
            //    {
            //        ReturnCustomerDto.Interests.Add(new UserInterestDto { /*Id = interest.InterestID,*/ Name = interest.InterestName, IsSelected = true });
            //    }
            //    else
            //    {
            //        ReturnCustomerDto.Interests.Add(new UserInterestDto { /*Id = interest.InterestID,*/ Name = interest.InterestName, IsSelected = false });
            //    }
            //}
            //foreach(var interest in customer.Interests)
            //{
            //    ReturnCustomerDto.Interests.Add(new UserInterestDto { Id = interest.InterestID, Name = interest.InterestName });
            //}
            ReturnCustomerDto.Interests = customer.Interests.Select(i => new UserInterestDto { Id = i.InterestID, Name = i.InterestName }).ToList();

            return ReturnCustomerDto;
        }
        public async Task<ResultDto> AddSource(string name)
        {

            var source = new Source
            {
                SourceName = name
            };
            var sources = await _unitOfWork.Sources.GetAllAsync();
            if (sources.Any(sources => sources.SourceName.ToLower() == name.ToLower()))
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Errors = ["Source already exists"]
                };
            }
            var result = await _unitOfWork.Sources.AddAsync(source);
            _unitOfWork.complete();
            BackgroundJob.Schedule(() => DeleteUnusedSource(source.SourceId), TimeSpan.FromDays(1));

            return new ResultDto
            {
                IsSuccess = true,
                Message = "Source added successfully"
            };
        }
        public async Task<ReturnAllCustomersDto> Search(string query, int page, int size)
        {
            //var customers = await _unitOfWork.Customers.GetAllAsync(c => c.FirstName.ToLower().Contains(query.ToLower()), ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            //if (!customers.Any())
            //{
            //    customers = await _unitOfWork.Customers.GetAllAsync(c => c.LastName.ToLower().Contains(query.ToLower()), ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            //}
            var customers = await _unitOfWork.Customers.GetAllAsync(c => (c.FirstName.ToLower() + " " + c.LastName.ToLower()).Contains(query.ToLower()) && !c.IsDeleted, ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            if (!customers.Any())
            {
                customers = await _unitOfWork.Customers.GetAllAsync(c => c.FirstName.ToLower().Contains(query.ToLower()) && !c.IsDeleted, ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            }
            if (!customers.Any())
            {
                customers = await _unitOfWork.Customers.GetAllAsync(c => c.LastName.ToLower().Contains(query.ToLower()) && !c.IsDeleted, ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            }
            if (!customers.Any())
            {
                customers = await _unitOfWork.Customers.GetAllAsync(c => c.Email.ToLower().Contains(query.ToLower()) && !c.IsDeleted, ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            }
            if (!customers.Any())
            {
                customers = await _unitOfWork.Customers.GetAllAsync(c => c.Phone.ToLower().Contains(query.ToLower()) && !c.IsDeleted, ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            }

            var customerResult = new List<ReturnCustomerDto>();
            foreach (var customer in customers)
            {
                var dto = new ReturnCustomerDto
                {
                    Id = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Age = customer.Age,
                    City = customer.City,
                    Email = customer.Email,
                    Gender = customer.Gender,
                    Phone = customer.Phone,
                    //SourceId = customer.Source.SourceId,
                    //SalesRepresntativeId = customer.SalesRepresntative.Id,
                    //Interests = customer.Interests.Select(i => new UserInterestDto { /*Id = i.InterestID,*/ Name = i.InterestName, IsSelected = true }).ToList(),
                    AdditionDate = customer.AdditionDate
                };
                var lastAction = await GetLastAction(customer.CustomerId);
                if (lastAction.Summary != null)
                {
                    dto.LastAction = lastAction;
                }
                dto.Source = new SourceDto
                {
                    Id = customer.Source.SourceId,
                    Name = customer.Source.SourceName
                };
                //foreach (var interest in customer.Interests)
                //{
                //    dto.Interests.Add(new UserInterestDto { Id = interest.InterestID, Name = interest.InterestName });
                //}
                dto.Interests = customer.Interests.Select(i => new UserInterestDto { Id = i.InterestID, Name = i.InterestName }).ToList();

                var userdto = new UserDto
                {
                    Id = customer.SalesRepresntative.Id,
                    FirstName = customer.SalesRepresntative.FirstName,
                    LastName = customer.SalesRepresntative.LastName,
                    UserName = customer.SalesRepresntative.UserName,
                    Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.SalesRepresntative),
                    Email = customer.SalesRepresntative.Email,
                    customers = await _unitOfWork.Customers.CountAsync(c => c.SalesRepresntative.Id == customer.SalesRepresntative.Id)
                };
                dto.SalesRepresentative = userdto;
                var userdto2 = new UserDto
                {
                    Id = customer.MarketingModerator.Id,
                    FirstName = customer.MarketingModerator.FirstName,
                    LastName = customer.MarketingModerator.LastName,
                    UserName = customer.MarketingModerator.UserName,
                    Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.MarketingModerator),
                    Email = customer.MarketingModerator.Email
                };
                dto.AddedBy = userdto2;
                customerResult.Add(dto);
            }
            var Customers = customerResult.OrderByDescending(DateTime => DateTime.AdditionDate).ToList();
            var CustomerPage = _filterService.Paginate(Customers, page, size);
            return new ReturnAllCustomersDto
            {
                IsSuccess = true,
                Pages = CustomerPage
                //Customers = customersDto.OrderByDescending(DateTime => DateTime.AdditionDate).ToList()
            };
        }
        public async Task<UserDto> GetSalesById(string id)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            var customersNumber = await _unitOfWork.Customers.CountAsync(c => c.SalesRepresntative.Id == id);
            if (user == null)
            {
                return null;
            }
            var userDto = new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Id = user.Id,
                customers = customersNumber
            };
            return userDto;
        }
        public async Task<ReturnAllCustomersDto> GetLastWeekCustomers(int page, int size)
        {
            var customers = await _unitOfWork.Customers.GetAllAsync(c => c.AdditionDate >= DateTime.Now.AddDays(-7) && !c.IsDeleted, ["Interests", "Source", "MarketingModerator", "SalesRepresntative"]);
            if (customers == null)
            {
                return new ReturnAllCustomersDto
                {
                    IsSuccess = false,
                    Errors = ["No customers found"]
                };
            }
            var customersDto = new List<ReturnCustomerDto>();
            foreach (var customer in customers)
            {
                var customerDto = new ReturnCustomerDto
                {
                    //CustomerId = customer.CustomerId,
                    Id = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    City = customer.City,
                    Age = customer.Age,
                    Gender = customer.Gender,
                    //SalesRepresntativeId = customer.SalesRepresntative.Id,
                    //SourceId = customer.Source.SourceId,
                    //Interests = customer.Interests.Select(i => new UserInterestDto { /*Id = i.InterestID,*/ Name = i.InterestName, IsSelected = true }).ToList(),
                    AdditionDate = customer.AdditionDate
                };
                customerDto.Source = new SourceDto
                {
                    Id = customer.Source.SourceId,
                    Name = customer.Source.SourceName
                };
                var lastAction = await GetLastAction(customer.CustomerId);
                if (lastAction.Summary != null)
                {
                    customerDto.LastAction = lastAction;
                }
                //foreach (var interest in customer.Interests)
                //{
                //    customerDto.Interests.Add(new UserInterestDto { Id = interest.InterestID, Name = interest.InterestName });
                //}
                customerDto.Interests = customer.Interests.Select(i => new UserInterestDto { Id = i.InterestID, Name = i.InterestName }).ToList();
                var userdto = new UserDto
                {
                    Id = customer.SalesRepresntative.Id,
                    FirstName = customer.SalesRepresntative.FirstName,
                    LastName = customer.SalesRepresntative.LastName,
                    UserName = customer.SalesRepresntative.UserName,
                    Roles = await _unitOfWork.UserManager.GetRolesAsync(customer.SalesRepresntative),
                    Email = customer.SalesRepresntative.Email,
                    customers = await _unitOfWork.Customers.CountAsync(c => c.SalesRepresntative.Id == customer.SalesRepresntative.Id)
                };
                customerDto.SalesRepresentative = userdto;
                var userdto2 = new UserDto
                {
                    Id = customer.MarketingModerator.Id,
                    FirstName = customer.MarketingModerator.FirstName,
                    LastName = customer.MarketingModerator.LastName,
                    UserName=customer.MarketingModerator.UserName,
                    Roles=await _unitOfWork.UserManager.GetRolesAsync(customer.MarketingModerator),
                    Email = customer.MarketingModerator.Email
                };
                customerDto.AddedBy = userdto2;
                customersDto.Add(customerDto);
            }
            var Customers = customersDto.OrderByDescending(DateTime => DateTime.AdditionDate).ToList();
            var CustomerPage = _filterService.Paginate(Customers, page, size);
            return new ReturnAllCustomersDto
            {
                IsSuccess = true,
                Pages = CustomerPage
                //Customers = customersDto.OrderByDescending(DateTime => DateTime.AdditionDate).ToList()
            };

        }
        public async Task<ActionDto> GetLastAction(int Id)
        {
            var customer = await _unitOfWork.Customers.FindAsync(c => c.CustomerId == Id, ["Messages","Calls","Meetings","Deals"]);
            var messages = await _unitOfWork.Messages.GetAllAsync();
            var lastMessage = messages.Where(m => m.Customer==customer).OrderByDescending(m => m.MessageDate).FirstOrDefault();
            var deals = await _unitOfWork.Deals.GetAllAsync();
            var lastDeal = deals.Where(d => d.Customer==customer).OrderByDescending(d=>d.DealDate).FirstOrDefault();
            var calls = await _unitOfWork.Calls.GetAllAsync();
            var lastCall = calls.Where(c => c.Customer == customer).OrderByDescending(c => c.CallDate).FirstOrDefault();
            var meetings = await _unitOfWork.Meetings.GetAllAsync();
            var lastMeeting = meetings.Where(m => m.Customer == customer).OrderByDescending(m => m.MeetingDate).FirstOrDefault();
        
            if (lastMessage == null && lastDeal == null && lastCall == null && lastMeeting == null)
            {
                return new ActionDto();
            }
        
            // comparise between the dates for the previous last actions and return the most recently action 
            var lastActions = new List<(DateTime Date, string Type, object Data)>
            {
                (lastMessage?.MessageDate ?? DateTime.MinValue, "Message", lastMessage),
                (lastDeal?.DealDate ?? DateTime.MinValue, "Deal", lastDeal),
                (lastCall?.CallDate ?? DateTime.MinValue, "Call", lastCall),
                (lastMeeting?.MeetingDate ?? DateTime.MinValue, "Meeting", lastMeeting)
            };
            
            var mostRecentAction = lastActions.OrderByDescending(a => a.Date).FirstOrDefault();
            
            if (mostRecentAction != default)
            {
                var lastAction = new ActionDto
                {
                    Date = mostRecentAction.Date,
                    Type = mostRecentAction.Type,
                };
                if (lastAction.Type == "Message")
                {
                    lastAction.Summary = lastMessage.MessageContent;
                    lastAction.Id = lastMessage.MessageID;
                }
                if (lastAction.Type == "Deal")
                {
                    lastAction.Summary = lastDeal.description;
                    lastAction.Id = lastDeal.DealId;
                }
                if (lastAction.Type == "Call")
                {
                    lastAction.Summary = lastCall.CallSummery;
                    lastAction.Id = lastCall.CallID;
                }
                if (lastAction.Type == "Meeting")
                {
                    lastAction.Summary = lastMeeting.MeetingSummary;
                    lastAction.Id = lastMeeting.MeetingID;
                }
                return lastAction;
            }
            else
            {
                return null; // or handle the case where no action is found
            }
        }
        public async Task<IEnumerable<ActionDto>> GetAllActionsForCustomer(int customerId)
        {
            var calls = await _unitOfWork.Calls.GetAllAsync(call => call.Customer.CustomerId == customerId);
            var messages = await _unitOfWork.Messages.GetAllAsync(message => message.Customer.CustomerId == customerId);
            var meetings = await _unitOfWork.Meetings.GetAllAsync(meeting => meeting.Customer.CustomerId == customerId);
            var deals = await _unitOfWork.Deals.GetAllAsync(deal => deal.Customer.CustomerId == customerId, includes: new[] { "Interest" });
            var interests = await _unitOfWork.Interests.GetAllAsync();


            var actions = calls.Select(call => new ActionDto
            {
                Id = call.CallID,
                Type = "call",
                Status = (int)call.CallStatus,
                Summary = call.CallSummery,
                Date = call.CallDate,
                FollowUp = call.FollowUpDate
            }).ToList();

            actions.AddRange(messages.Select(message => new ActionDto
            {
                Id = message.MessageID,
                Type = "message",
                Summary = message.MessageContent,
                Date = message.MessageDate,
                FollowUp = message.FollowUpDate
            }));

            actions.AddRange(meetings.Select(meeting => new ActionDto
            {
                Id = meeting.MeetingID,
                Type = "meeting",
                Online = meeting.connectionState,
                Summary = meeting.MeetingSummary,
                Date = meeting.MeetingDate,
                FollowUp = meeting.FollowUpDate
            }));
            actions.AddRange(deals.Select(deal => new ActionDto
            {
                Id = deal.DealId,
                Type = "deal",
                Price = deal.Price,
                Interest = interests.FirstOrDefault(i => i.InterestID == deal.Interest.InterestID)?.ToInterestDto(), // Map Interest entity to InterestDto
                Summary = deal.description,
                Date = deal.DealDate
            }));


            return actions;
        }
        public async Task DeleteUnusedSource(int id)
        {
            var source =await _unitOfWork.Sources.FindAsync(x=>x.SourceId==id);
            if(source != null&& !source.Customers.Any())
            {
                _unitOfWork.Sources.Delete(id);
                _unitOfWork.complete();
            }
        }

    }
}
