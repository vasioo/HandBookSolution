using HandBook.Models.BaseModels.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Models
{
    public class AppUser : IdentityUser,IUser
    {
        public string Gender { get; set; } = "";
        public virtual ICollection<Messages> Messages { get; set; }
    }
}
