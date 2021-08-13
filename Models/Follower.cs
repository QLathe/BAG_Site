using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// This is the relationship that is created when
// the Artist is followed by the Follower
namespace models.Models
{
    public class Followers
    {
        [Key]
        public int LinkId {get;set;}
        public int FollowerId {get;set;}
        public User Follower {get;set;}
        public int UserId {get;set;}
    }
}