using HandBook.Web.Models;
using Messenger.Models;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandBook.Models
{
    public class Likes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime LikedDate { get; set; }

        // Navigation properties
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("AppUser")]
        public string UserId { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Post")]
        public int PostId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Comment")]
        public int CommentId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; } = new AppUser();

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("PostId")]
        public virtual Post Post { get; set; } = new Post();


    }
}
