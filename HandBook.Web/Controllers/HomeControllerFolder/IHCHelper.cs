using HandBook.Models;
using HandBook.Web.Models;
using Messenger.Models;

namespace HandBook.Web.Controllers.HomeControllerFolder
{
    public interface IHCHelper
    {
        Task<IQueryable<Comment>> LoadMoreCommentsHelper(int offset, int derivingFrom, int postId);
        Task<List<CardDTO>> LoadMorePostsHelper(int offset);
        Task AddAPostHelper(Post tfm, IFormFile ImageUrl, Notification ntf, AppUser user);
        Task<string> AddOrRemoveACommentHelper(CommentsDTO commentsDTO, AppUser user);
        Task<int> IncrementOrDecrementLikeCountHelper(int itemId, AppUser user);
        Task<int> IncrementOrDecrementCommentLikeCountHelper(int itemId, AppUser user);
        Task<UserAccountDTO> AccountHelper(AppUser user, AppUser currUser);
        Task AddFollowerRelationshipHelper(string username, string usernamefollower);
        Task RemoveFollowerRelationshipHelper(string username, string usernamefollower);
        Task<List<CardDTO>> NotificationsHelper(AppUser user);
        Task<Post> DesiredPostHelper(int desiredPostId);
        Task<List<CardDTO>> IndexHelper(AppUser user);
    }
}
