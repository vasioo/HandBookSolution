
using HandBook.Models;
using HandBook.Web.Models;
using Messenger.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HandBook.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>()
                .Property(m => m.image).HasColumnType("varbinary(max)");

            builder.Entity<Likes>()
            .HasKey(l => l.Id);

            builder.Entity<Likes>()
               .HasOne(l => l.AppUser)
               .WithMany()
               .HasForeignKey(l => l.UserId);

            builder.Entity<Likes>()
                .HasOne(l => l.Post)
                .WithMany()
                .HasForeignKey(l => l.PostId);

            builder.Entity<Comment>()
            .HasKey(l => l.Id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          
        }
    }
}