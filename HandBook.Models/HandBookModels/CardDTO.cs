using HandBook.Models;

namespace HandBook.Web.Models
{
    public class CardDTO
    {
        public int Id { get; set; }

        public string CreatorUserName { get; set; } = "";

        public int AmountOfLikes { get; set; }

        public string image { get; set; } = "";

        public DateTime Time { get; set; } = DateTime.Now;

        public int AmountOfComments { get; set; }

        public List<int> UserLikedCards { get; set; }= new List<int>();
        public string UserLikedComments { get; set; } = "";

        public string Description { get; set; } = "";
    }
}
