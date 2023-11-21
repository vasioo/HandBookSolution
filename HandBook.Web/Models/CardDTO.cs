namespace HandBook.Web.Models
{
    public class CardDTO
    {
        public int Id { get; set; }

        public string CreatorUserName { get; set; } = "";

        public int AmountOfLikes { get; set; }

        public byte[] image { get; set; } = new byte[0];

        public DateTime Time { get; set; } = DateTime.Now;

        public int AmountOfComments { get; set; }
    }
}
