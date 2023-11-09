using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HandBook.Data;
using Messenger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceStack.DataAnnotations;

namespace HandBook.Models
{
    public class Post:IPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Unique]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string CreatorUserName { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public DateTime Time { get; set; }=DateTime.Now;

        [System.ComponentModel.DataAnnotations.Required]
        //counting the amount of likes and one person can only like once
        public int AmountOfLikes { get; set; } = 0;

        //public List<string> Comments { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        //if saved it turns to true
        //it can be saved in any time
        public bool Saved { get; set; } = false;

        //public virtual AppUser Creator { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public byte[] image { get; set; }

        [NotMapped]
        public bool IsLiked { get; set; }

    }
}
