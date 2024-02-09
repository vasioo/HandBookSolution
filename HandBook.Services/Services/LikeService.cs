using HandBook.DataAccess;
using HandBook.Models;
using HandBook.Services.Interfaces;
using HandBook.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandBook.Services.Services
{
    public class LikeService : BaseService<Likes>, ILikesService
    {
        private readonly ApplicationDbContext _dataContext;
        public LikeService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }

        public IQueryable<Likes> GetUserLikedComments(string userId)
        {
            var data = _dataContext.Likes.Where(com => com.AppUser.Id == userId && com.CommentId != Guid.Empty);
            return data;
        }

        public IQueryable<Likes> GetUserLikedPosts(string userId)
        {
            var data = _dataContext.Likes.Where(like => like.UserId == userId && like.CommentId == Guid.Empty);
            return data;
        }

        public Likes GetLikeEntityForUserAndCommentInfo(string userId, Guid itemId)
        {
            var data = _dataContext.Likes.Where(x => x.AppUser.Id == userId&& x.CommentId == itemId).FirstOrDefault();
            return data;
        }

        public Likes GetLikeEntityForUserAndPostInfo(string userId, Guid itemId)
        {
            var data = _dataContext.Likes.Where(x => x.AppUser.Id == userId && x.Post.Id == itemId).FirstOrDefault();
            return data;
        }
    }
}
