using HandBook.Models;
using HandBook.Web.Models;
using Messenger.Models;

namespace HandBook.Services.Interfaces
{
    public interface IPostService : IBaseService<Post>
    {
        IQueryable<CardDTO> GetPostsBasedOnCreatorUser(string creatorUserUsername);
        IQueryable<CardDTO> GetPostsBasedOnUserFavouritism(AppUser user);
    }
}
