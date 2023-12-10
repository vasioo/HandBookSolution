using HandBook.DataAccess;
using HandBook.Services.Interfaces;
using HandBook.Web.Models;

namespace HandBook.Services.Services
{
    public class FollowerService : BaseService<Followers>, IFollowerService
    {
        private readonly ApplicationDbContext _dataContext;
        public FollowerService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }

        public Task<int> GetFollowerCount(string userId)
        {
            var data = _dataContext.Followers.Where(p => p.FollowedUserId == userId).Count();
            return Task.FromResult(data);
        }

        public Task<int> GetFollowedCount(string userId)
        {
            var data = _dataContext.Followers.Where(p => p.FollowerUserId == userId).Count();
            return Task.FromResult(data);
        }

        public Task<bool> FindIfUserIsFollowed(string userId,string currUserId)
        {
            var data = _dataContext.Followers.Any(f => f.FollowerUserId == currUserId && f.FollowedUserId == userId);
            return Task.FromResult(data);
        }
    }
}
