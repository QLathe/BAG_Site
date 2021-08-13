using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace models.ViewModels
{
    public class ProfileViewModel
    {
        
        [Required]
        public IFormFile Image {get;set;}
    }

}