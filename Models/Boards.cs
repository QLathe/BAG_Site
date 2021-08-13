using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace models.Models 
{
    public class Board 
    {
        [Key]
        public int BoardId {get;set;}
        [Required]
        public string Title {get;set;}
        public string Description {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
        // Viewable: 0 - private, 1 - invite-only, 2 - public
        [Display(Name ="Viewability")]
        public int Viewable {get;set;} = 0;
        public List<Pin> Pinned {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}