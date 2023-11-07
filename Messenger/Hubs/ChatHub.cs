using Messenger.Controllers;
using Messenger.Data;
using Messenger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Hubs
{
    public class ChatHub:Hub
    {
        private readonly ILogger<MessagesController> _logger;

        public readonly MessengerDbContext _context;

        public readonly UserManager<AppUser> _userManager;


        public ChatHub(MessengerDbContext context, ILogger<MessagesController> logger, UserManager<AppUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task SendMessage(string user,string message,string targetUser)
        {
            await Clients.All.SendAsync("receiveMessage",user, message);
            AppUser appuser = await _userManager.FindByNameAsync(user);
            var chatMessage = new Messages
            {
                UserId = appuser.Id,
                Text = message,
                TimeSent = DateTime.Now,
                Username = user,
                ReceiverId = targetUser,
            };

            await _context.Messages.AddAsync(chatMessage);
            await _context.SaveChangesAsync();
        }
    }
}
