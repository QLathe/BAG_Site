using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    // The link between User and the Art they like
    public class Job
    {
        [Key]
        public int JobId {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
        public int ArtistId {get;set;}
        [Required]
        public string Title {get;set;}
        [Required]
        public string Description {get;set;}
        
        [EmailAddress]
        public string Email {get;set;}
        public int Status {get;set;} = 0;
        // Status. 0: Offer sent, 1: In progress, 2: Complete, 3: Rejected, 4: Disputed
        public int Pay {get;set;}
        public string Company {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}