using HandBook.Models;
using HandBook.Web.Models;

namespace HandBook.Services.Interfaces
{
    public interface IFollowerService : IBaseService<Followers>
    {
        Task<int> GetFollowerCount(string userId);
        Task<int> GetFollowedCount(string userId);
        Task<bool> FindIfUserIsFollowed(string userId, string currUserId);

    }
}
