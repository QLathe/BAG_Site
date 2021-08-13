using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    // The link between User and the Art they like
    public class Bagitem
    {
        [Key]
        public int BagitemId {get;set;}
        public int ArtId {get;set;}
        public Art Art {get;set;}
        public int BagId {get;set;}
        public Bag bag {get;set;}
    }
}