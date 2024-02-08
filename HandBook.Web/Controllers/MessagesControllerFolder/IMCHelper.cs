using Messenger.Models;

namespace HandBook.Web.Controllers.MessagesControllerFolder
{
    public interface IMCHelper
    {
        Task CreateMessage(Messages message, AppUser user);
        List<Messages> GetUnreadMessagesForUser(string userId, string targetUserId);
        List<string> GetUsersSentMessagesTo(string userId);
        IOrderedQueryable<Messages> GetCurrentChatMessages(string currentUserId, string otherUserId);
        Task<List<UserMassageDTO>> GetUsersWithMessages(string senderId);
    }
}
