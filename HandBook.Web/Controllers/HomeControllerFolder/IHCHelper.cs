using HandBook.Models;
using HandBook.Models.JSONModel;
using HandBook.Models.ViewModels;
using HandBook.Web.Models;
using Messenger.Models;

namespace HandBook.Web.Controllers.HomeControllerFolder
{
    public interface IHCHelper
    {
        IQueryable<CommentJSON> LoadMoreCommentsHelper(int offset, Guid derivingFrom, Guid postId);
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
        IQueryable<CardDTO> FeedHelper(AppUser user);
        Task<ExplorePageViewModel> GetExplorePageAttributes(AppUser user);
        IQueryable<CardDTO> GetSpecificExplorePageItemsByProvidedItemHelper(Guid itemId);
    }
}
