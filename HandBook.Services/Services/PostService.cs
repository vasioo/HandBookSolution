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
    public class PostService: BaseService<Post>, IPostService
    {
        private readonly ApplicationDbContext _dataContext;
        public PostService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }

        public IQueryable<CardDTO> GetPostsBasedOnCreatorUser(string creatorUserUsername)
        {
            var posts = _dataContext.Posts.Where(x => x.CreatorUserName == creatorUserUsername);

            var cardDTO = posts.OrderByDescending(x => x.Time)
                .Take(20)
                .Select(post => new CardDTO
                {
                    Id = post.Id,
                    CreatorUserName = post.CreatorUserName,
                    AmountOfLikes = post.AmountOfLikes,
                    image = post.ImageLink,
                    Time = post.Time,
                    AmountOfComments = post.Comments.Count()
                });

            return cardDTO;
        }
    }
}
