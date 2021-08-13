using System;  
using System.Collections.Generic;  
using System.Text;  
using Microsoft.EntityFrameworkCore;

namespace models.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<Board> Boards {get;set;}
        public DbSet<User> Users {get;set;}
        public DbSet<Bagitem> Bagitems {get;set;}
        public DbSet<Bag> Bag {get;set;}
        public DbSet<Art> Art {get;set;}
        public DbSet<Tag> Tags {get;set;}
        public DbSet<Liked> Likes {get;set;}
        public DbSet<Skill> Skills {get;set;}
        public DbSet<Pin> Pins {get;set;}
        public DbSet<FArtist> FArtists {get;set;}
        public DbSet<Followers> Followers {get;set;}
        public DbSet<Job> Jobs {get;set;}
        public DbSet<Transaction> Transactions {get;set;}
        public DbSet<Comment> Comments {get;set;}
    }
}