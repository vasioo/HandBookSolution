using Messenger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HandBook.Web.Controllers.MessagesControllerFolder
{
    public class MCHelper
    {
        public readonly UserManager<AppUser> _userManager;

        public MCHelper(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public List<string> GetUsersSentMessagesTo(string userId)
        {
            var receiverIds = _context.Messages.OrderByDescending(x => x.TimeSent)
                .Where(m => m.SenderMessageId == userId)
                .Select(m => m.MessageReceiverId)
                .Distinct()
            .ToList();

            var users = _context.Users
                .Where(u => receiverIds.Contains(u.Id))
                .Select(u => u.UserName)
                .ToList();

            return users!;
        }
        public List<Messages> GetUnreadMessagesForUser(string userId, string targetUserId)
        {
            var unreadMessages = _context.Messages
                .Where(m => m.MessageReceiverId == userId && m.SenderMessageId == targetUserId && !m.IsRead)
                .ToList();
            return unreadMessages;
        }
        public async Task CreateMessage(Messages message,AppUser user)
        {
            message.Username = user!.UserName!;
            message.SenderMessageId = user!.Id!;
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
    }
}
