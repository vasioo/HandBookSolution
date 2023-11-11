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
         
        }

        
    }
}