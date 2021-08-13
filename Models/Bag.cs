using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    // The link between User and the Art they like
    public class Bag
    {
        [Key]
        public int BagId {get;set;}
        public List<Bagitem> Art {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
    }
}