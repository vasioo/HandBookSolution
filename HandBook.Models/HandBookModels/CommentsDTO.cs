namespace HandBook.Web.Models
{
    public class CommentsDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PostId { get; set; } = Guid.NewGuid();
        public string UserUsername { get; set; } = "";
        public string CommentContent { get; set; } = "";
        public DateTime DateOfCreation { get; set; } = DateTime.Now;
        public Guid CommentDeriveFromId { get; set; } = new Guid();
    }
}
