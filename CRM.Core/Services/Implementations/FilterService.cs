using CRM.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Services.Implementations
{
    public class FilterService:IFilterService
    {
        public PagesDto<T> Paginate<T>(IEnumerable<T> source, int page = 1, int size = 10) where T : class
        {
            if (page == 0)
            {
                page = 1;
            }
            if (size == 0)
            {
                size = 10;
            }
            var total = source.Count();
            var Pages = (int)Math.Ceiling(total / (decimal)size);
            var Items = source.Skip((page - 1) * size).Take(size);

            //    var result = new
            //    {
            //        Pages,
            //        Items,
            //        CurrentPage= page
            //    };
            //    return result;
            //
            var pagesDto = new PagesDto<T>
            {
                CurrentPage = page,
                Items = Items,
                Pages = Pages,
                TotalItems = total,
                PageItems = Items.Count()
            };
            return pagesDto;
        }
        public class PagesDto<T> where T : class 
        {
            public IEnumerable<T> Items { get; set; }
            public int Pages { get; set; }
            public int CurrentPage { get; set; }
            public int TotalItems { get; set; }
            public int PageItems { get; set; }
        }
    }
}
