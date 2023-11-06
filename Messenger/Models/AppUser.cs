using Microsoft.AspNetCore.Identity;

namespace Messenger.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Messages = new List<Messages>();
        }

        public virtual ICollection<Messages> Messages { get; set; }
    }
}
