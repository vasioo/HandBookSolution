using Messenger.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Messenger.Data
{
    public class MessengerDbContext : IdentityDbContext
    {
        public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            builder.Entity<Message>()
                .HasOne<AppUser>(au => au.Sender)
                .WithMany(d => d.Messages)
                .HasForeignKey(d => d.UserId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-HandBook-7364a21d-fa2f-416f-a2ea-fd49efd1b593;Trusted_Connection=True;MultipleActiveResultSets=true");

        }

        public DbSet<Message> Message{ get; set; }
    }
}