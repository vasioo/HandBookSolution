using HandBook.Models;
using Messenger.Models;

namespace HandBook.Web.Models
{
    public class UserAccountDTO
    {
        public AppUser UserTemp { get; set; }

        public IEnumerable<Post> PostsTemp { get; set; }
    }
}
