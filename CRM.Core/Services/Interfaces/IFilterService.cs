using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CRM.Core.Services.Implementations.FilterService;

namespace CRM.Core.Services.Interfaces
{
    public interface IFilterService
    {
        PagesDto<T> Paginate<T>(IEnumerable<T> source, int page = 1, int size = 10) where T : class;
    }
}
