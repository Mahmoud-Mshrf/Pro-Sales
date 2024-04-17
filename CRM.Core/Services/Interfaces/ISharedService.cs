using CRM.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Interfaces
{
    public interface ISharedService
    {
        Task<ReturnInterstsDto> GetAllInterests();
        Task<ReturnSourcesDto> GetAllSources();
        Task<BusinessDto> GetBussinesInfo();
        Task<IEnumerable<object>> GetActionsForCustomer(int customerId);
    }
}
