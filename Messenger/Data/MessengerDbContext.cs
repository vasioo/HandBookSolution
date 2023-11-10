using Messenger.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;

namespace Messenger.Data
{
    public class MessengerDbContext : IdentityDbContext<AppUser>
    {
        private bool seedDb;
        public MessengerDbContext(DbContextOptions<MessengerDbContext> options, bool seedDb = true)
            : base(options)
        {
            this.seedDb = seedDb;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-HandBook-7364a21d-fa2f-416f-a2ea-fd49efd1b593;Trusted_Connection=True;MultipleActiveResultSets=true");

        }

        public DbSet<Messages> Messages { get; set; }
    }
}