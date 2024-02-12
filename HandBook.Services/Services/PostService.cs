using HandBook.DataAccess;
using HandBook.Models;
using HandBook.Services.Interfaces;
using HandBook.Web.Models;
using Messenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandBook.Services.Services
{
    public class PostService : BaseService<Post>, IPostService
    {
        private readonly ApplicationDbContext _dataContext;
        public PostService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }

        public IQueryable<CardDTO> GetPostsBasedOnCreatorUser(string creatorUserUsername)
        {
            var posts = _dataContext.Posts.Where(x => x.CreatorUserName == creatorUserUsername);
            var comments = _dataContext.Comments;

            var cardDTO = posts.OrderByDescending(x => x.Time)
                .Take(20)
                .Select(post => new CardDTO
                {
                    Id = post.Id,
                    CreatorUserName = post.CreatorUserName,
                    AmountOfLikes = post.AmountOfLikes,
                    image = post.ImageLink,
                    Time = post.Time,
                    AmountOfComments = comments.Where(x=>x.Post.Id==post.Id).Count()
                });

            return cardDTO;
        }

        public IQueryable<CardDTO> GetPostsBasedOnUserFavouritism(AppUser user)
        {
            var posts = _dataContext.Posts;
            var coms = _dataContext.Comments;

            var cardDTO = posts.OrderByDescending(x => x.Time)
                .Select(post => new CardDTO
                {
                    Id = post.Id,
                    CreatorUserName = post.CreatorUserName,
                    AmountOfLikes = post.AmountOfLikes,
                    image = post.ImageLink,
                    Time = post.Time,
                    AmountOfComments = coms.Where(x=>x.Post.Id==post.Id).Count()
                });

            return cardDTO;
        }
    }
}
