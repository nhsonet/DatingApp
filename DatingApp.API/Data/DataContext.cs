using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Value> Value { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Photo> Photo { get; set; }
        public DbSet<Like> Like { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>()
                   .HasKey(k => new { k.LikerId, k.LikeeId });

            builder.Entity<Like>()
                   .HasOne(k => k.Likee)
                   .WithMany(k => k.Likers)
                   .HasForeignKey(k => k.LikeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                   .HasOne(k => k.Liker)
                   .WithMany(k => k.Likees)
                   .HasForeignKey(k => k.LikerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}