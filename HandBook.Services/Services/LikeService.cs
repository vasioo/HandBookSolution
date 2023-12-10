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

        public Task<IQueryable<Likes>> GetUserLikedComments(string userId)
        {
            var data = _dataContext.Likes.Where(com => com.AppUser.Id == userId && com.CommentId != 0);
            return Task.FromResult(data);
        }

        public Task<IQueryable<Likes>> GetUserLikedPosts(string userId)
        {
            var data = _dataContext.Likes.Where(like => like.UserId == userId && like.CommentId == 0);
            return Task.FromResult(data);
        }

        public Likes GetLikeEntityForUserAndCommentInfo(string userId,int itemId)
        {
            var data = _dataContext.Likes.Where(x => x.AppUser.Id == userId&& x.CommentId == itemId).FirstOrDefault();
            return data;
        }

        public Likes GetLikeEntityForUserAndPostInfo(string userId, int itemId)
        {
            var data = _dataContext.Likes.Where(x => x.AppUser.Id == userId && x.Post.Id == itemId).FirstOrDefault();
            return data;
        }
    }
}
