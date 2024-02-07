using Messenger.Models;

namespace HandBook.Models.BaseModels.Interfaces
{
    public interface IUser
    {
        string Gender { get; set; }
        ICollection<Messages> Messages { get; set; }
    }
}
