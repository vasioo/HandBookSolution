using HandBook.Models;

namespace HandBook.Services.Interfaces
{
    public interface INotificationService:IBaseService<Notification>
    {
        Task<IOrderedQueryable<Notification>> GetNotificationsByUserId(string userId);
        Task<Notification> GetExistingNotification(string userId, int postId, string reason);
    }
}
