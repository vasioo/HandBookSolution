using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using System.Diagnostics;

namespace HandBook.Web.Controllers.MessagesControllerFolder
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ILogger<MessagesController> _logger;

        public readonly UserManager<AppUser> _userManager;

        public IMCHelper _helper { get; set; }

        public MessagesController(ILogger<MessagesController> logger, UserManager<AppUser> userManager, IMCHelper helper)
        {
            _logger = logger;
            _userManager = userManager;
            _helper = helper;
        }

        [Authorize]
        public async Task<IActionResult> Index(int currentPage)
        {
            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            List<UserMassageDTO> userMsgDTOList = new List<UserMassageDTO>();
            var username = HttpContext.User?.Identity?.Name ?? "";
            var sender = await _userManager.FindByNameAsync(username);
            if (sender != null)
            {
                var neededUsers = GetUsersSentMessagesTo(sender.Id);

                foreach (var item in neededUsers)
                {
                    UserMassageDTO userMsgDto = new UserMassageDTO();

                    userMsgDto.UserData = item;

                    var specUserNeeded = _context.Users.Where(u => u.UserName == item).FirstOrDefault();

                    if (specUserNeeded != null)
                    {
                        var messages = GetUnreadMessagesForUser(sender.Id, specUserNeeded.Id);

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
                   .Where(m => m.SenderMessageId == sender.Id && m.MessageReceiverId == specUserNeeded.Id || m.SenderMessageId == specUserNeeded.Id && m.MessageReceiverId == sender.Id)
                   .OrderByDescending(m => m.TimeSent)
                   .FirstOrDefault();

                            userMsgDto.Message = lastMessage!.Text!;

                        }
                    }
                    userMsgDTOList.Add(userMsgDto);
                }

                return View("~/MessengerViews/Messages/Index.cshtml", userMsgDTOList.Take(20).Skip((currentPage - 1) * 20));
            }
            return View("~/MessengerViews/Messages/Index.cshtml");
        }

        [Authorize]
        public async Task<IActionResult> Chat(int currentPage, string userName)
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
                    .Where(m => m.SenderMessageId == currentUser.Id && m.MessageReceiverId == otherUserId || m.SenderMessageId == otherUserId && m.MessageReceiverId == currentUser.Id)
                    .OrderBy(m => m.TimeSent)
                    .ToListAsync();


                await _context.SaveChangesAsync();

                return View("~/MessengerViews/Messages/Chat.cshtml", messages);
            }
            return View("~/MessengerViews/Messages/Index.cshtml");
        }

        [HttpPost]
        [Authorize]
        public async Task Create(Messages message)
        {
            await _helper.CreateMessage(message,User);
        }

        #region Helpers

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View("~/MessengerViews/Messages/Privacy.cshtml");
        }
        #endregion
    }
}