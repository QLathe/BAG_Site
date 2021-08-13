using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace models.Models 
{
    // Pins are the link between a User's Board and the Art that is saved on it
    public class Pin
    {
        [Key]
        public int PinId {get;set;}
        public int BoardId {get;set;}
        public Board Board {get;set;}
        public int ArtId {get;set;}
        public Art Art {get;set;}
        public string Note {get;set;}
    }
}