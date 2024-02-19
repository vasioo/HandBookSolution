using HandBook.Models.BaseModels.Interfaces;
using Messenger.Models;

namespace HandBook.Models.UserModel
{
    public class BannedUser:IEntity
    {
        public Guid Id { get; set; }

        public AppUser Sender { get; set; } = new AppUser();
        public AppUser Receiver { get; set; } = new AppUser();
    }
}
