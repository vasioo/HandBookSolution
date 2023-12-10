using HandBook.Models;
using HandBook.Web.Models;

namespace HandBook.Services.Interfaces
{
    public interface IPostService : IBaseService<Post>
    {
        IQueryable<CardDTO> GetPostsBasedOnCreatorUser(string creatorUserUsername);
    }
}
