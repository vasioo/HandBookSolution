namespace HandBook.Models
{
    public interface IPost
    {

        public int Id { get; set; }

        public string CreatorUserName { get; set; }

        public DateTime Time { get; set; }

        public int AmountOfLikes { get; set; }

        public bool Saved { get; set; }

        public byte[] image { get; set; }
    }
}
