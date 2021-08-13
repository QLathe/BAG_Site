using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// This is the relationship that is created when
// the User follows an artist (Target)
namespace models.Models
{
    public class FArtist
    {
        [Key]
        public int LinkId {get;set;}
        public int TargetId {get;set;}
        public User Target {get;set;}
        public int UserId {get;set;}
    }
}