using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DatingApp.API.Data
{
       public class DataContext : IdentityDbContext<User, Role, int,
              IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
       {
              public DataContext(DbContextOptions<DataContext> options) : base(options)
              {
              }

              public DbSet<Value> Values { get; set; }
              
              // public DbSet<User> Users { get; set; }

              public DbSet<Photo> Photos { get; set; }
              public DbSet<Like> Likes { get; set; }
              public DbSet<Message> Messages { get; set; }

              protected override void OnModelCreating(ModelBuilder builder)
              {
                     base.OnModelCreating(builder);

                     builder.Entity<UserRole>(userRole =>
                     {
                            userRole.HasKey(k => new { k.UserId, k.RoleId });

                            userRole.HasOne(k => k.User)
                                    .WithMany(k => k.UserRole)
                                    .HasForeignKey(k => k.UserId)
                                    .IsRequired();

                            userRole.HasOne(k => k.Role)
                                    .WithMany(k => k.UserRole)
                                    .HasForeignKey(k => k.RoleId)
                                    .IsRequired();
                     });

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

                     builder.Entity<Message>()
                            .HasOne(k => k.Sender)
                            .WithMany(k => k.MessagesSent)
                            .OnDelete(DeleteBehavior.Restrict);

                     builder.Entity<Message>()
                            .HasOne(k => k.Recipient)
                            .WithMany(k => k.MessagesReceived)
                            .OnDelete(DeleteBehavior.Restrict);

                     builder.Entity<Photo>().HasQueryFilter(k => k.IsApproved);
              }
       }
}