using Messenger.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HandBook.Web.Models
{
    public class Followers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FollowerUserId { get; set; }
        public int FollowedUserId { get; set; }

        public AppUser Follower { get; set; } = new AppUser();
        public AppUser Followed { get; set; } = new AppUser();  
    }
}
