using Messenger.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HandBook.Models.BaseModels.Interfaces;

namespace HandBook.Web.Models
{
    public class Followers:IEntity
    {
        public int Id { get; set; }

        public string FollowerUserId { get; set; } = "";
        public string FollowedUserId { get; set; } = "";

        public AppUser Follower { get; set; } = new AppUser();
        public AppUser Followed { get; set; } = new AppUser();  
    }
}
