﻿using Messenger.Controllers;
using Messenger.Data;
using Messenger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Hubs
{
    public class ChatHub : Hub
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

        public async Task SendMessage(string user, string message, string targetUser)
        {
            AppUser appuser = await _userManager.FindByNameAsync(user);
            await Clients.User(targetUser).SendAsync("receiveMessage", user, message, false);
            await Clients.User(appuser.Id).SendAsync("receiveMessage", user, message,true);

            var chatMessage = new Messages
            {
                SenderMessageId = appuser.Id,
                Text = message,
                TimeSent = DateTime.Now,
                Username = user,
                MessageReceiverId = targetUser
            };

            await _context.Messages.AddAsync(chatMessage);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsRead(string user, string targetUser)
        {
            await _context.Messages.Where(m => m.Username == user && m.MessageReceiverId == targetUser).ForEachAsync(m => m.IsRead = true);
            await _context.SaveChangesAsync();
        }
    }
}
