using HandBook.Models;
using Messenger.Models;

namespace HandBook.Services.Interfaces
{
    public interface IMessageService : IBaseService<Messages>
    {
        List<string> UsersThatAreInMessagesList(string userId);
        List<string> UsersThatHaveSentAMessage(string userId);
    }
}
