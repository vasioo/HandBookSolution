using HandBook.Models;

namespace HandBook.Services.Interfaces
{
    public interface ILikesService : IBaseService<Likes>
    {
        IQueryable<Likes> GetUserLikedPosts(string userId);
        IQueryable<Likes> GetUserLikedComments(string userId);
        Likes GetLikeEntityForUserAndPostInfo(string userId, Guid itemId);
        Likes GetLikeEntityForUserAndCommentInfo(string userId, Guid itemId);
    }
}
