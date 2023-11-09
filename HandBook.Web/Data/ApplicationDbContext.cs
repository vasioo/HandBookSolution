using HandBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HandBook.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
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

            builder.Entity<Likes>().HasNoKey();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                 .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-HandBook-7364a21d-fa2f-416f-a2ea-fd49efd1b593;Trusted_Connection=True;MultipleActiveResultSets=true");

        }
    }
}