using Messenger.Data;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var receiverIds = _context.Messages
                .Where(m => m.UserId == userId)
                .Select(m => m.ReceiverId)
                .Distinct()
                .ToList();

            var users = _context.Users
                .Where(u => receiverIds.Contains(u.Id))
                .Select(u => u.UserName)
                .ToList();

            return users;
        }

        public List<Messages> GetUnreadMessagesForUser(string userId)
        {
            var unreadMessages = _context.Messages
                .Where(m => m.ReceiverId == userId && !m.IsRead)
                .ToList();

            return unreadMessages;
        }

        public async Task<IActionResult> Index()
        {
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
                    var messages = GetUnreadMessagesForUser(specUserNeeded.Id);

                    if (messages.Count == 1)
                    {
                        userMsgDto.Message = messages.FirstOrDefault().Text;
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
               .Where(m => (m.UserId == sender.Id && m.ReceiverId == specUserNeeded.Id) || (m.UserId == specUserNeeded.Id && m.ReceiverId == sender.Id))
               .OrderByDescending(m => m.TimeSent)
               .FirstOrDefault();

                        userMsgDto.Message = lastMessage.Text;

                    }
                }
                userMsgDTOList.Add(userMsgDto);
            }

            return View(userMsgDTOList);
        }

        public async Task<IActionResult> Chat(string userName)
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var specUserNeeded = _context.Users.Where(u => u.UserName == userName).FirstOrDefault();
                ViewBag.CurrentUserName = currentUser.UserName;

                var otherUserId = specUserNeeded.Id;
                var messages = await _context.Messages
                    .Where(m => (m.UserId == currentUser.Id && m.ReceiverId == otherUserId) || (m.UserId == otherUserId && m.ReceiverId == currentUser.Id))
                    .OrderBy(m => m.TimeSent)
                    .ToListAsync();

                foreach (var message in messages)
                {
                    message.IsRead = true;
                }

                await _context.SaveChangesAsync();

                return View(messages);
            }
            return View(Index);
        }

        [HttpPost]
        public async Task Create(Messages message)
        {
            message.Username = User.Identity.Name;
            var sender = await _userManager.GetUserAsync(User);
            message.UserId = sender.Id;
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