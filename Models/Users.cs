using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        public string DisplayName {get;set;}

        [Required]
        public string FirstName {get;set;}
        [Required]
        public string LastName {get;set;}
        [Required]
        [EmailAddress]
        public string Email {get;set;}

        [Required]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer")]
        [RegularExpression("^((?=.*?[A-Za-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at least one letter, one number and one special character (e.g. !@#$%^&*)")]
        
        [DataType(DataType.Password)]
        public string Password {get;set;}
        
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
        public string ProfilePic {get;set;}
        // for the profile pic
        public DateTime DoB {get;set;}
        // Not sure why we need this. Might get rid of it.
        public int Mentor {get;set;} = 0;
        // Possibly will create a link in the future for Mentor/Mentee relations, for now: 
        // 0 - none, 1 - looking for a Mentor, 2 - looking to Mentor
        public int OpenForCommission {get;set;} = 0;
        // 0 - not open for commission, 1 - open for commission
        public int UserLvl {get;set;} = 0;
        // This will track if the User has verified their email. Possibly can store subscription level here as well. 0 - un-verified, 1 - verified
        public string Bio {get;set;}
        public int Subscription {get;set;} = 0;
        // Shows the subscription level of the User. 0 - none, 1 - first tier, etc
        public Bag Bag {get;set;}
        public List<Skill> Skills {get;set;}
        public List<Transaction> Purchases {get;set;}
        // User can see their purchase History
        public List<Sold> SellingHistory {get;set;}
        // Artist will be able to see the art they've sold
        public List<FArtist> FavArtist {get;set;}
        public List<Followers> Followers {get;set;}
        // Favorite Artists: acts kind of like following them
        public List<Liked> LikedArt {get;set;}
        public List<Art> Art {get;set;}
        public List<Comment> Comments {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    }
}