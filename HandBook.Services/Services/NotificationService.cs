using HandBook.DataAccess;
using HandBook.Models;
using HandBook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var followers = _dataContext.Followers.Where(f => f.FollowedUserId == userId);

            var followersId = followers.Select(f => f.FollowerUserId);

            var data = _dataContext.Notifications.Where(card => followersId.Contains(card.UserId)).OrderByDescending(x => x.Time);

            return Task.FromResult(data);
        }

        public Task<Notification> GetExistingNotification(string userId, int postId, string reason)
        {
            if (postId>0)
            {
                var tempdata = _dataContext.Notifications.Where(x => x.UserId == userId && x.MainText == reason && x.Post.Id == postId).FirstOrDefault();

                return Task.FromResult(tempdata);
            }
            var data = _dataContext.Notifications.Where(x => x.UserId == userId && x.MainText == reason).FirstOrDefault();

            return Task.FromResult(data);
        }
    }
}
