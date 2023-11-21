using Messenger.Models;

namespace HandBook.Web.Models
{
    public class Followers
    {
        public int FollowerUserId { get; set; }
        public int FollowedUserId { get; set; }

        public AppUser Follower { get; set; } = new AppUser();
        public AppUser Followed { get; set; } = new AppUser();  
    }
}
