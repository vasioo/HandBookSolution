using HandBook.Models;
using HandBook.Models.ViewModels;
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
        Task<IOrderedQueryable<Notification>> NotificationsHelper(AppUser user);
        Task<CardDTO> DesiredPostHelper(int desiredPostId, AppUser user);
        Task<List<CardDTO>> IndexHelper(AppUser user);
        Task<ExplorePageViewModel> GetExplorePageAttributes(AppUser user);
    }
}
