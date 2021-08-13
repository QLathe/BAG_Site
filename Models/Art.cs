using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;  
using System.Threading.Tasks;

namespace models.Models
{
    public class Art
    {
        [Key]
        public int ArtId {get;set;}
        public string Title {get;set;}
        public int Feature {get;set;} = 0;
        [Required]
        public int CreatorId {get;set;}
        public User Creator {get;set;}
        public int price_data {get;set;}
        public string ImgFile {get;set;}
        public string Genre {get;set;}
        public List<Tag> Tags  {get;set;}
        public List<Liked> LikedBy {get;set;}
        public List<Comment> Comments {get;set;}
        public DateTime UploadDate {get;set;} = DateTime.Now;
    }

}