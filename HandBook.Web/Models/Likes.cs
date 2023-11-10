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

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("AppUser")]
        public string UserId { get; set; } = "";
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Post")]
        public int PostId { get; set; }
        public DateTime LikedDate { get; set; }

        // Navigation properties
        public AppUser AppUser { get; set; } = new AppUser();
        public Post Post { get; set; }=new Post();
    }
}
