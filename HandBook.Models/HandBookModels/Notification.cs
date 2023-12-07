using HandBook.Models.BaseModels.Interfaces;
using Messenger.Models;

namespace HandBook.Models
{
    public class Notification:IEntity
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string CreatorUserName { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Required]
        public DateTime Time { get; set; } = DateTime.Now;

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Post")]
        public int? PostId { get; set; }

        public string MainText { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("AppUser")]
        public string? UserId { get; set; } = "";

        public AppUser? AppUser { get; set; } = new AppUser();
        public Post? Post { get; set; } = new Post();
    }
}
