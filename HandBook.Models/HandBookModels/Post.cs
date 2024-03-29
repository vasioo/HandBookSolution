﻿using HandBook.Models.BaseModels.Interfaces;
using HandBook.Web.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace HandBook.Models
{
    public class Post:IEntity
    {
        public Guid Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string CreatorUserName { get; set; } = "";

        [System.ComponentModel.DataAnnotations.Required]
        public DateTime Time { get; set; } = DateTime.Now;

        [System.ComponentModel.DataAnnotations.Required]
        public int AmountOfLikes { get; set; } 

        [System.ComponentModel.DataAnnotations.Required]
        public string ImageLink { get; set; } = "";

        [NotMapped]
        public bool IsLiked { get; set; } = false;

        //if using a # it is counted as a tag which can be clicked onto and it shows other posts with similar items
        public string Description { get; set; } = "";
        

    }
}
