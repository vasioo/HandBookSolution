using HandBook.Models;
using HandBook.Models.BaseModels.Interfaces;
using Messenger.Models;

namespace HandBook.Web.Models
{
    public class Comment:IEntity
    {
        public Guid Id { get; set; }

        public DateTime DateOfCreation { get; set; }

        public string CommentContent { get; set; } = "";

        // Navigation properties
        public AppUser AppUser { get; set; } = new AppUser();
        public Post Post { get; set; } = new Post();

        public int AmountOfLikes { get; set; }

        public Guid CommentDeriveFromId { get; set; }
    }
}
