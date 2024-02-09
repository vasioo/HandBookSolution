using HandBook.Models.BaseModels.Interfaces;
using Messenger.Models;
using System.Linq.Expressions;

namespace HandBook.Services.Interfaces
{
    public interface IBaseService<T> where T : IEntity
    {
        Task<Guid> AddAsync(T entity);
        Task<int> AddRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> IQueryableGetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<int> RemoveAsync(Guid id);
        Task<int> RemoveRangeAsync(IEnumerable<T> entities);
        Task<int> UpdateAsync(T entity);
        Task<int> UpdateRangeAsync(IEnumerable<T> entity);
        Task<int> GetCountOfAllItems();
        IQueryable<AppUser> IQueryableGetUsersThatAreWorkers();
    }
}
