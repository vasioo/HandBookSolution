using HandBook.Models.UserModel;
using HandBook.Services.Interfaces;
using Messenger.Models;
using Microsoft.AspNetCore.Identity;

namespace HandBook.Web.Controllers.MessagesControllerFolder
{
    public class MCHelper : IMCHelper
    {

        #region FieldsAndController
        public readonly UserManager<AppUser> _userManager;

        private readonly IMessageService _messageService;
        private readonly IFollowerService _followerService;
        private readonly IBannedUserService _bannedUserService;

        public MCHelper(UserManager<AppUser> userManager, IMessageService messageService, IFollowerService followerService, IBannedUserService bannedUserService)
        {
            _userManager = userManager;
            _messageService = messageService;
            _followerService = followerService;
            _bannedUserService = bannedUserService;

        }
        #endregion

        #region MainPage

        public async Task<List<UserMassageDTO>> GetUsersWithMessages(string senderId)
        {
            var neededUsersThatHaveAFollowingBinding = GetUsersSentMessagesTo(senderId);
            var neededUsersThatDontContainAFollowingBinding = GetUsersThatSentAMessage(senderId).Except(neededUsersThatHaveAFollowingBinding);

            List<UserMassageDTO> userMsgDTOList = new List<UserMassageDTO>();

            foreach (var item in neededUsersThatHaveAFollowingBinding)
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
                        userMsgDto.DateOfSending = messages!.FirstOrDefault()!.TimeSent;
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
                        userMsgDto.DateOfSending = messages!.OrderByDescending(x => x.TimeSent)!.Select(x => x.TimeSent).FirstOrDefault();
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
                        userMsgDto.DateOfSending = lastMessage.TimeSent;
                    }
                }
                userMsgDTOList.Add(userMsgDto);
            }

            foreach (var item in neededUsersThatDontContainAFollowingBinding)
            {
                UserMassageDTO userMsgDto = new UserMassageDTO();

                userMsgDto.UserData = item;

                var users = _userManager.Users;

                var specUserNeeded = users.Where(u => u.UserName == item).FirstOrDefault();

                if (specUserNeeded != null)
                {
                    var messages = GetUnreadMessagesForUser(senderId, specUserNeeded.Id);

                    if (messages.Count > 0)
                    {

                        userMsgDto.Message = $"*new request";
                        userMsgDto.DateOfSending = messages!.OrderByDescending(x => x.TimeSent)!.Select(x => x.TimeSent).FirstOrDefault();
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
                        userMsgDto.DateOfSending = lastMessage.TimeSent;
                    }
                }

                userMsgDTOList.Add(userMsgDto);
            }

            return userMsgDTOList;
        }

        #endregion

        #region Chat
        public async Task CreateMessage(Messages message, AppUser user)
        {
            message.Username = user!.UserName!;
            message.SenderMessageId = user!.Id!;
            await _messageService.AddAsync(message);
        }

        public List<string> GetUsersSentMessagesTo(string userId)
        {
            return _messageService.UsersThatAreInMessagesList(userId);
        }

        private List<string> GetUsersThatSentAMessage(string userId)
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

        public IOrderedQueryable<Messages> GetCurrentChatMessages(string currentUserId, string otherUserId)
        {
            var messages = _messageService.IQueryableGetAllAsync()
                .Where(m => (m.SenderMessageId == currentUserId && m.MessageReceiverId == otherUserId)
                || (m.SenderMessageId == otherUserId && m.MessageReceiverId == currentUserId))
                .OrderBy(m => m.TimeSent);

            return messages;
        }

        public bool IsThereAreRelationshipBetweenThem(AppUser sender, AppUser receiver)
        {
            var relationship = _followerService.IQueryableGetAllAsync()
                .Where(x => x.Follower.Id == sender.Id && x.Followed.Id == receiver.Id)
                .First();

            var senderMessages = GetUsersSentMessagesTo(sender.Id);
            var receiverMessages = GetUsersThatSentAMessage(sender.Id);

            var messagesFirstlySentToSender = receiverMessages.Except(senderMessages).ToList();

            var isConnection = messagesFirstlySentToSender.Contains(receiver.Id);

            if (relationship.Id == Guid.Empty)
            {
                if (!isConnection)
                {
                    return false;
                }
                return false;
            }
            return true;
        }

        public bool BanAUser(AppUser sender, AppUser receiver)
        {
            var existingModel = _bannedUserService.IQueryableGetAllAsync().Where(x => x.Sender == sender && x.Receiver == receiver).FirstOrDefault();
            if (existingModel == null)
            {
                var model = new BannedUser();
                model.Sender = sender;
                model.Receiver = receiver;
                _bannedUserService.AddAsync(model);
                return true;
            }
            return false;
        }
        #endregion

    }
}
