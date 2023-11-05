using Messenger.Data;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }
            var messages = await _context.Message.ToListAsync();
            return View(messages);
        }

        [HttpPost]
        public async Task Create(Message message)
        {
            message.Username = User.Identity.Name;
            var sender = await _userManager.GetUserAsync(User);
            message.UserId = sender.Id;
            await _context.Message.AddAsync(message);
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