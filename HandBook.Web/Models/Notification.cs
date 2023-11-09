using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HandBook.Data;
using Microsoft.AspNetCore.Mvc;

namespace HandBook.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Unique]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string CreatorUserName { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public DateTime Time { get; set; } = DateTime.Now;

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Post")]
        public int PostId { get; set; }
    }
}
