using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    // Might not need this. Might just use the transactions model to create sell history as well
    public class Sold
    {
        [Key]
        public int SoldId {get;set;}
        public int UserId {get;set;}
        public User User {get;set;}
        public int Price {get;set;}
        public DateTime PurchaseDate {get;set;} = DateTime.Now;
        public bool Refund {get;set;} = false;
    }
}