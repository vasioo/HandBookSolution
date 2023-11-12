﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HandBook.Web.Models;
using Messenger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceStack.DataAnnotations;

namespace HandBook.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Unique]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string CreatorUserName { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Required]
        public DateTime Time { get; set; }=DateTime.Now;

        [System.ComponentModel.DataAnnotations.Required]
        //counting the amount of likes and one person can only like once
        public int AmountOfLikes { get; set; } = 0;

        public List<Comment> Comments { get; set; }

        //public virtual AppUser Creator { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public byte[] image { get; set; } = new byte[0];

        [NotMapped]
        public bool IsLiked { get; set; } = false;

    }
}
