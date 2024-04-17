using CRM.Core.Dtos;
using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Interfaces
{
    public interface IModeratorService
    {
        // Will be used after adding Manager module
        Task<ReturnUsersDto> GetAllSalesRepresentatives();
        Task<ReturnCustomerDto> AddCustomer(AddCustomerDto customerDto, string marketingModeratorEmail);
        //Task<ResultDto> AddCustomer(CustomerDto customerDto, string marketingModeratorEmail);
        Task<ReturnCustomerDto> UpdateCustomer(AddCustomerDto customerDto, int customerId);
        Task<ReturnCustomerDto> GetCustomer(int customerId);
        Task<ReturnAllCustomersDto> GetAllCustomers(int page, int size);
        Task<ReturnAllCustomersDto> GetLastWeekCustomers(int page, int size);
        //Task<ReturnAllCustomersDto> GetAllCustomers();
        Task<ResultDto> DeleteCustomer(int CustomerId);
        Task<ResultDto> AddSource(string name);
        Task<ReturnAllCustomersDto> Search(string query,int page,int size);
        //Task<IEnumerable<ReturnCustomerDto>> Search(string query);
        Task<UserDto> GetSalesById(string id);
        Task<IEnumerable<object>> GetAllActionsForCustomer(int customerId);
        Task<ActionDto> GetLastAction(int Id);
        
    }
}
