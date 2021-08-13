using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    // Comments that User can leave on uploaded art
    public class Comment
    {
        [Key]
        public int CommentId {get;set;}
        public string Message {get;set;}
        public int ArtId {get;set;}
        public Art Art {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
        public DateTime WrittenOn {get;set;} = DateTime.Now;
    }
}