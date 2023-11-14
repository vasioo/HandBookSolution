using HandBook.Models;
using Messenger.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HandBook.Web.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UniqueIdentifier{ get; set; }

        public DateTime DateOfCreation { get; set; }

        public string CommentContent { get; set; }

        // Navigation properties
        public AppUser AppUser { get; set; } = new AppUser();
        public Post Post { get; set; } = new Post();

        public IEnumerable<Likes> LikesOnComment { get; set; }

        public int CommentDeriveFromId { get; set; }
    }
}
