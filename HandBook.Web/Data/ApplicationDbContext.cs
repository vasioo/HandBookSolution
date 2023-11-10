
using HandBook.Models;
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

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                 .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-HandBook-7364a21d-fa2f-416f-a2ea-fd49efd1b593;Trusted_Connection=True;MultipleActiveResultSets=true");

        }
    }
}