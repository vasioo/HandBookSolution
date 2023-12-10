using HandBook.Models;

namespace HandBook.Services.Interfaces
{
    public interface ILikesService : IBaseService<Likes>
    {
        Task<IQueryable<Likes>> GetUserLikedPosts(string userId);
        Task<IQueryable<Likes>> GetUserLikedComments(string userId);
        Likes GetLikeEntityForUserAndPostInfo(string userId, int itemId);
        Likes GetLikeEntityForUserAndCommentInfo(string userId, int itemId);
    }
}
