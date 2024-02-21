using HandBook.DataAccess;
using HandBook.Models;
using HandBook.Services.Interfaces;

namespace HandBook.Services.Services
{
    public class NotificationService : BaseService<Notification>, INotificationService
    {
        private readonly ApplicationDbContext _dataContext;
        public NotificationService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }

        public Task<IOrderedQueryable<Notification>> GetNotificationsByUserId(string userId)
        {
            var followers = _dataContext.Followers.Where(f => f.Follower.Id == userId);

            var followersId = followers.Select(f => f.Followed.Id);

            var data = _dataContext.Notifications.Where(card => followersId.Contains(card.UserId)).OrderByDescending(x => x.Time);

            return Task.FromResult(data);
        }

        public Task<Notification> GetExistingNotification(string userId, Guid postId, string reason)
        {
            if (postId!=Guid.Empty)
            {
                var tempdata = _dataContext.Notifications.Where(x => x.UserId == userId && x.MainText == reason && x.Post.Id == postId).FirstOrDefault();

                return Task.FromResult(tempdata);
            }
            var data = _dataContext.Notifications.Where(x => x.UserId == userId && x.MainText == reason).FirstOrDefault();

            return Task.FromResult(data);
        }
    }
}
