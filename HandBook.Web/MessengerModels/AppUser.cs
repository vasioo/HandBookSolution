using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Messenger.Models
{
    public class AppUser : IdentityUser
    {
        public string Gender { get; set; } = "";
        public virtual ICollection<Messages> Messages { get; set; }
    }
}
