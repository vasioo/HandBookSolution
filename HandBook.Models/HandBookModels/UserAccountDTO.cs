using HandBook.Models;
using Messenger.Models;

namespace HandBook.Web.Models
{
    public class UserAccountDTO
    {
        public string UserTempUsername { get; set; }

        public IEnumerable<CardDTO> PostsTemp { get; set; }

        public IEnumerable<Likes>UserLikedCards { get; set; }
        public string UserLikedComments{ get; set; }
    }
}
