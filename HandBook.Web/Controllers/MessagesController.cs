using Messenger.Data;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ServiceStack;
using System.Diagnostics;

namespace Messenger.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ILogger<MessagesController> _logger;

        public readonly MessengerDbContext _context;

        public readonly UserManager<AppUser> _userManager;

        public MessagesController(MessengerDbContext context, ILogger<MessagesController> logger, UserManager<AppUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public List<string> GetUsersSentMessagesTo(string userId)
        {
            var receiverIds = _context.Messages.OrderByDescending(x=>x.TimeSent)
                .Where(m => m.SenderMessageId == userId)
                .Select(m => m.MessageReceiverId)
                .Distinct()
                .ToList();

            var users = _context.Users
                .Where(u => receiverIds.Contains(u.Id))
                .Select(u => u.UserName)
                .ToList();

            return users;
        }

        public List<Messages> GetUnreadMessagesForUser(string userId,string targetUserId)
        {
            var unreadMessages = _context.Messages
                .Where(m => m.MessageReceiverId == userId && m.SenderMessageId == targetUserId && !m.IsRead)
                .ToList();
            return unreadMessages;
        }

        public async Task<IActionResult> Index(int currentPage)
        {
            if (currentPage<=0)
            {
                currentPage = 1;
            }

            List<UserMassageDTO> userMsgDTOList = new List<UserMassageDTO>();
            var sender = await _userManager.GetUserAsync(User);
            var neededUsers = GetUsersSentMessagesTo(sender.Id);

            foreach (var item in neededUsers)
            {
                UserMassageDTO userMsgDto = new UserMassageDTO();

                userMsgDto.UserData = item;

                var specUserNeeded = _context.Users.Where(u => u.UserName == item).FirstOrDefault();

                if (specUserNeeded != null)
                {
                    var messages = GetUnreadMessagesForUser(sender.Id,specUserNeeded.Id);

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
                        var lastMessage = _context.Messages
               .Where(m => (m.SenderMessageId == sender.Id && m.MessageReceiverId == specUserNeeded.Id) || (m.SenderMessageId == specUserNeeded.Id && m.MessageReceiverId == sender.Id))
               .OrderByDescending(m => m.TimeSent)
               .FirstOrDefault();

                        userMsgDto.Message = lastMessage!.Text!;

                    }
                }
                userMsgDTOList.Add(userMsgDto);
            }

            return View(userMsgDTOList.Take(20).Skip((currentPage-1)*20));
        }

        public async Task<IActionResult> Chat(int currentPage,string userName)
        {
            if (currentPage <= 0)
            {
                currentPage = 1;
            }
            if (User!.Identity!.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var specUserNeeded = _context.Users.Where(u => u.UserName == userName).FirstOrDefault();
                ViewBag.CurrentUserName = currentUser.UserName;

                var otherUserId = specUserNeeded!.Id;
                var messages = await _context.Messages
                    .Where(m => (m.SenderMessageId == currentUser.Id && m.MessageReceiverId == otherUserId) || (m.SenderMessageId == otherUserId && m.MessageReceiverId == currentUser.Id))
                    .OrderBy(m => m.TimeSent)
                    .ToListAsync();


                await _context.SaveChangesAsync();

                return View(messages);
            }
            return View("");
        }

        [HttpPost]
        public async Task Create(Messages message)
        {
            message.Username = User!.Identity!.Name!;
            var sender = await _userManager.GetUserAsync(User);
            message.SenderMessageId = sender.Id;
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}