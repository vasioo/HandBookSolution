using HandBook.Models.BaseModels.Interfaces;
using Messenger.Models;

namespace HandBook.Web.Models
{
    public class Followers:IEntity
    {
        public Guid Id { get; set; }

        public AppUser Follower { get; set; } = new AppUser();
        public AppUser Followed { get; set; } = new AppUser();  
    }
}
