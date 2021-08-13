using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models.Models
{
    public class Tag
    {
        [Key]
        public int TagId {get;set;}
        public string Desc {get;set;}
        public int ArtId {get;set;}
        public Art Art {get;set;}
    }
}