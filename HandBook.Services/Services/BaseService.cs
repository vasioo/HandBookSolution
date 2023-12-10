using HandBook.DataAccess;
using HandBook.Models.BaseModels.Interfaces;
using HandBook.Services.Interfaces;
using Messenger.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HandBook.Services.Services
{
    public class BaseService<T> : IBaseService<T> where T : class, IEntity
    {
        private readonly ApplicationDbContext _context;

        public BaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();

            // Access the primary key value and return it
            return entity.Id;
        }

        public async Task<int> AddRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            await _context.SaveChangesAsync();

            // Return the number of entities added
            return entities.Count();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public IQueryable<T> IQueryableGetAllAsync()
        {
            return _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity == null)
                {
                    throw new ArgumentException("No entity was found with that id");
                }
                return entity;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> GetCountOfAllItems()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<int> RemoveAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
                return 0;

            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            if (!_context.Set<T>().Local.Any(e => e.Id == entity.Id))
            {
                _context.Set<T>().Attach(entity);
            }
            _context.Entry(entity).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;

            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
            return await _context.SaveChangesAsync();
        }

        public IQueryable<AppUser> IQueryableGetUsersThatAreWorkers()
        {
            var users = (from user in _context.Users
                         join userRole in _context.UserRoles
                         on user.Id equals userRole.UserId
                         join role in _context.Roles
                         on userRole.RoleId equals role.Id
                         where role.Name == "WORKER"
                         select user);
            return users;
        }
    }
}
