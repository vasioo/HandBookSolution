using HandBook.Models;
using HandBook.Web.Models;

namespace HandBook.Web.Controllers.HomeControllerFolder
{
    public interface IHCHelper
    {
        Task<IQueryable<Comment>> LoadMoreCommentsHelper(int offset, int derivingFrom, int postId);
        Task<List<CardDTO>> LoadMorePostsHelper(int offset);
    }
}
