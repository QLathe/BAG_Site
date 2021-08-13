using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId {get;set;}
        public int BuyerId {get;set;}
        public User Buyer {get;set;}
        public int SellerId {get;set;}
        public int Price {get;set;}
        public DateTime PurchaseDate {get;set;} = DateTime.Now;
    }
}