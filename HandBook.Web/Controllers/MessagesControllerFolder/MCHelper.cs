using HandBook.Services.Interfaces;
using Messenger.Models;
using Microsoft.AspNetCore.Identity;

namespace HandBook.Web.Controllers.MessagesControllerFolder
{
    public class MCHelper : IMCHelper
    {
        public readonly UserManager<AppUser> _userManager;

        private readonly IMessageService _messageService;

        public MCHelper(UserManager<AppUser> userManager, IMessageService messageService)
        {
            _userManager = userManager;
            _messageService = messageService;
        }
        public List<string> GetUsersSentMessagesTo(string userId)
        {
            return _messageService.UsersThatAreInMessagesList(userId);
        }
        public List<Messages> GetUnreadMessagesForUser(string userId, string targetUserId)
        {
            var allMessages = _messageService.IQueryableGetAllAsync();

            var unreadMessages = allMessages
                .Where(m => m.MessageReceiverId == userId && m.SenderMessageId == targetUserId && !m.IsRead)
                .ToList();
            return unreadMessages;
        }
        public async Task CreateMessage(Messages message, AppUser user)
        {
            message.Username = user!.UserName!;
            message.SenderMessageId = user!.Id!;
            await _messageService.AddAsync(message);
        }

        public IOrderedQueryable<Messages> GetCurrentChatMessages(string currentUserId, string otherUserId)
        {
            var messages = _messageService.IQueryableGetAllAsync()
                .Where(m => (m.SenderMessageId == currentUserId && m.MessageReceiverId == otherUserId) 
                || (m.SenderMessageId == otherUserId && m.MessageReceiverId == currentUserId))
                .OrderBy(m => m.TimeSent);

            return messages;
        }


        public async Task<List<UserMassageDTO>> GetUsersWithMessages(string senderId)
        {
            var neededUsers = GetUsersSentMessagesTo(senderId);
            List<UserMassageDTO> userMsgDTOList = new List<UserMassageDTO>();
            foreach (var item in neededUsers)
            {
                UserMassageDTO userMsgDto = new UserMassageDTO();

                userMsgDto.UserData = item;

                var users = _userManager.Users;

                var specUserNeeded = users.Where(u => u.UserName == item).FirstOrDefault();

                if (specUserNeeded != null)
                {
                    var messages = GetUnreadMessagesForUser(senderId, specUserNeeded.Id);

                    if (messages.Count == 1)
                    {
                        userMsgDto.Message = messages!.FirstOrDefault()!.Text;
                    }
                    else if (messages.Count > 0)
                    {
                        int neededCountNumber = messages.Count();
                        if (neededCountNumber > 4)
                        {
                            userMsgDto.Message = $"4+ unopened messages";
                        }
                        else
                        {
                            userMsgDto.Message = $"{messages.Count()} unopened messages";
                        }
                    }
                    else
                    {
                        var allMessages = _messageService.IQueryableGetAllAsync();

                        var lastMessage = allMessages
                            .Where(m => m.SenderMessageId == senderId && m.MessageReceiverId == specUserNeeded.Id
                            || m.SenderMessageId == specUserNeeded.Id && m.MessageReceiverId == senderId)
                            .OrderByDescending(m => m.TimeSent)
                            .FirstOrDefault();

                        userMsgDto.Message = lastMessage!.Text!;

                    }
                }
                userMsgDTOList.Add(userMsgDto);
            }
            return userMsgDTOList;
        }
    }
}
