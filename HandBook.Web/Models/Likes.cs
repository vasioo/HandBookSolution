using HandBook.Data;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandBook.Models
{
    public class Likes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Unique]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("User")]
        public string UserId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Post")]
        public int PostId { get; set; }
        public DateTime LikedDate { get; set; }
    }
}
