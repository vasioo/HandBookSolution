using HandBook.Models;
using Messenger.Models;

namespace HandBook.Web.Models
{
    public class UserAccountDTO
    {
        public string UserTempUsername { get; set; }

        public IEnumerable<CardDTO> PostsTemp { get; set; }

        public IEnumerable<int>UserLikedCards { get; set; }
        public string UserLikedComments{ get; set; }

        public int Follows { get; set; }
        public int Followers { get; set; }
        public bool FollowsThePerson { get; set; }
        public bool IsTheSamePerson { get; set; }
    }
}
