using CRM.Core.Consts;
using CRM.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core.Interfaces
{
    // IBaseRepository = IDynamicRepository = IGenericRepository
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindAsync(Expression<Func<T, bool>> predicate, string[] includes = null);// includes is an optional parameter
        Task<T> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, string[] includes = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, int take, int skip);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, int take, int skip, string[] includes = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, int? take, int? skip,
            Expression<Func<T, object>> orderBy, string[] includes = null, string orderByDirection = OrderBy.Ascending);
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        T Update(T entity);
        IEnumerable<T> UpdateRange(IEnumerable<T> entities);
        void Delete(int id);
        void Delete(T entity);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync(string[] includes);
        
    }
}
