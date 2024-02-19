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
            var data = _dataContext.Followers.Where(p => p.Followed.Id == userId).Count();
            return Task.FromResult(data);
        }

        public Task<int> GetFollowedCount(string userId)
        {
            var data = _dataContext.Followers.Where(p => p.Follower.Id == userId).Count();
            return Task.FromResult(data);
        }

        public Task<bool> FindIfUserIsFollowed(string userId,string currUserId)
        {
            var data = _dataContext.Followers.Any(f => f.Follower.Id == currUserId && f.Followed.Id == userId);
            return Task.FromResult(data);
        }
    }
}
