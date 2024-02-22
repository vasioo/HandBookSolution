using HandBook.Models.BaseModels.Interfaces;
using Messenger.Models;

namespace HandBook.Models
{
    public class Notification:IEntity
    {
        public Guid Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string CreatorUserName { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Required]
        public DateTime Time { get; set; } = DateTime.Now;

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Post")]
        public Guid? PostId { get; set; }

        public string MainText { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("AppUser")]
        public string? UserId { get; set; } = "";

        public AppUser? AppUser { get; set; } 
        public Post? Post { get; set; } 
    }
}
