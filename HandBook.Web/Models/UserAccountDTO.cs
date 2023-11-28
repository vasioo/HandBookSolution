using HandBook.Models;
using Messenger.Models;

namespace HandBook.Web.Models
{
    public class UserAccountDTO
    {
        public string UserTempUsername { get; set; }

        public IEnumerable<CardDTO> PostsTemp { get; set; }
    }
}
