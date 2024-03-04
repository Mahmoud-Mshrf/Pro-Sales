using CRM.Core.Dtos;
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
        Task<ResultDto> AddCustomer(CustomerDto customerDto, string marketingModeratorEmail);
        Task<ResultDto> UpdateCustomer(CustomerDto customerDto, int CustomerId);
        Task<ReturnCustomerDto> GetCustomer(int CustomerId, string moderatorEmail);
    }
}
