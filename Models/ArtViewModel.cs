using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace models.ViewModels
{
    public class ArtViewModel
    {
        [Required]
        public string Title {get;set;}
        public int Feature {get;set;} = 0;
        [Required]
        public int Price {get;set;}
        [Required]
        public string Genre {get;set;}
        
        [Required]
        public IFormFile Image {get;set;}
    }

}