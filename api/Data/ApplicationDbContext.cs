using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) 
        : base(dbContextOptions)
        {
            
        }

        public DbSet<User> Users {get; set;}
        public DbSet<Wishlist> Wishlists {get; set;}
        public DbSet<History> Histories {get; set;}
        public DbSet<Impression> Impressions {get; set;}
        public DbSet<Comment> Comments {get; set;}
        public DbSet<Series> Series {get; set;}
        public DbSet<SeriesCategory> SeriesCategories {get; set;}
        public DbSet<TagCategory> TagCategories {get; set;}
        public DbSet<Category> Categories {get; set;}
        public DbSet<Tag> Tags {get; set;}
        public DbSet<Episode> Episodes {get; set;}
        public DbSet<Video> Videos {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Episode>()
            .HasOne(e => e.Series)
            .WithMany(e => e.Episodes)
            .HasForeignKey(e => e.SeriesId);
       
            modelBuilder.Entity<Episode>()
            .HasOne(e => e.Video)
            .WithOne(e => e.Episode)
            .HasForeignKey<Episode>(e => e.VideoId);

             modelBuilder.Entity<Series>()
            .Property(s => s.SeriesFormat)
            .HasConversion<string>();
        }
        
    }
}