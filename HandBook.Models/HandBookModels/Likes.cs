using HandBook.Models.BaseModels.Interfaces;
using Messenger.Models;

namespace HandBook.Models
{
    public class Likes:IEntity
    {
        public Guid Id { get; set; }

        public DateTime LikedDate { get; set; }

        // Navigation properties
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("AppUser")]
        public string UserId { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Post")]
        public Guid PostId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Comment")]
        public Guid CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; } = new AppUser();

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("PostId")]
        public virtual Post Post { get; set; } = new Post();


    }
}
