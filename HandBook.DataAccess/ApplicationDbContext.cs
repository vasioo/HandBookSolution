using HandBook.Models;
using HandBook.Models.UserModel;
using HandBook.Web.Models;
using Messenger.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HandBook.DataAccess
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
        public DbSet<Followers> Followers { get; set; }
        public DbSet<BannedUser> BannedUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Post>()
                .Property(m => m.ImageLink).HasColumnType("varbinary(max)");

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

            builder.Entity<Comment>()
                .HasOne(c => c.Post);


            builder.Entity<Comment>().Navigation(e => e.Post).AutoInclude();
            builder.Entity<Comment>().Navigation(e => e.AppUser).AutoInclude();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          
        }
    }
}