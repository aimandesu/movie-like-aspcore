using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }

        public override DbSet<User> Users { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Impression> Impressions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<SeriesCategory> SeriesCategories { get; set; }
        public DbSet<TagCategory> TagCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Video> Videos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<Episode>()
            // .HasOne(e => e.Series)
            // .WithMany(e => e.Episodes)
            // .HasForeignKey(e => e.SeriesId);

            // modelBuilder.Entity<Episode>()
            // .HasOne(e => e.Video)
            // .WithOne(e => e.Episode)
            // .HasForeignKey<Episode>(e => e.VideoId);

            modelBuilder.Entity<Series>()
           .Property(s => s.SeriesFormat)
           .HasConversion<string>();

            // modelBuilder.Entity<SeriesCategory>()
            // .HasOne(sc => sc.Series)
            // .WithMany(s => s.SeriesCategories)
            // .HasForeignKey(sc => sc.SeriesId);

            // modelBuilder.Entity<SeriesCategory>()
            // .HasOne(sc => sc.Category)
            // .WithMany()
            // .HasForeignKey(sc => sc.CategoryId);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER"
                }
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }

    }
}