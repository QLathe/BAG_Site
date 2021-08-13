using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    // The link between User and the Art they like
    public class Liked
    {
        [Key]
        public int LikeId {get;set;}
        public int ArtId {get;set;}
        public Art Art {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
        public DateTime LikedAt {get;set;} = DateTime.Now;
    }
}