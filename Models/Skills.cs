using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    // User can add unique skills for their profile and save them
    public class Skill
    {
        [Key]
        public int SkillId {get;set;}
        public string Desc {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
    }
}