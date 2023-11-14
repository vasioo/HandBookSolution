using HandBook.Models;
using Messenger.Models;

namespace HandBook.Web.Models
{
    public class CommentsDTO
    {
        public int Id { get; set; } = 0;
        public int PostId { get; set; } = 0;
        public string AppUsername { get; set; } = "";
        public string CommentContent { get; set; } = "";
        public DateTime DateOfCreation { get; set; } = DateTime.Now;
        public int CommentDeriveFromId { get; set; } = 0;
    }
}
