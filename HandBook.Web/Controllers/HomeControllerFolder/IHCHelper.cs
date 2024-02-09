using HandBook.Models;
using HandBook.Models.ViewModels;
using HandBook.Web.Models;
using Messenger.Models;

namespace HandBook.Web.Controllers.HomeControllerFolder
{
    public interface IHCHelper
    {
        Task<IQueryable<Comment>> LoadMoreCommentsHelper(int offset, Guid derivingFrom, Guid postId);
        Task<List<CardDTO>> LoadMorePostsHelper(int offset);
        Task AddAPostHelper(Post tfm, IFormFile ImageUrl, Notification ntf, AppUser user);
        Task<string> AddOrRemoveACommentHelper(CommentsDTO commentsDTO, AppUser user);
        Task<int> IncrementOrDecrementLikeCountHelper(Guid itemId, AppUser user);
        Task<int> IncrementOrDecrementCommentLikeCountHelper(Guid itemId, AppUser user);
        Task<UserAccountDTO> AccountHelper(AppUser user, AppUser currUser);
        Task AddFollowerRelationshipHelper(string username, string usernamefollower);
        Task RemoveFollowerRelationshipHelper(string username, string usernamefollower);
        Task<IOrderedQueryable<Notification>> NotificationsHelper(AppUser user);
        Task<CardDTO> DesiredPostHelper(Guid desiredPostId, AppUser user);
        IQueryable<CardDTO> IndexHelper(AppUser user);
        Task<ExplorePageViewModel> GetExplorePageAttributes(AppUser user);
    }
}
