using HandBook.DataAccess;
using HandBook.Models;
using HandBook.Services.Interfaces;
using Messenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandBook.Services.Services
{
    public class MessageService : BaseService<Messages>, IMessageService
    {
        private readonly ApplicationDbContext _dataContext;
        public MessageService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }

        public List<string> UsersThatAreInMessagesList(string userId)
        {
            var receiverIds = _dataContext.Messages.OrderByDescending(x => x.TimeSent)
               .Where(m => m.SenderMessageId == userId)
               .Select(m => m.MessageReceiverId)
               .Distinct()
           .ToList();

            var users = _dataContext.Users
                .Where(u => receiverIds.Contains(u.Id))
                .Select(u => u.UserName)
                .ToList();

            return users!;
        }
    }
}
