using HandBook.Models;
using HandBook.Services.Interfaces;
using HandBook.Web.Models;
using Messenger.Models;
using Microsoft.AspNetCore.Identity;

namespace HandBook.Web.Controllers.HomeControllerFolder
{
    public class HCHelper : IHCHelper
    {
        public readonly UserManager<AppUser> _userManager;

        private readonly ICommentService _commentService;
        private readonly IPostService _postService;

        public HCHelper(UserManager<AppUser> userManager, ICommentService commentService, IPostService postService)
        {
            _userManager = userManager;
            _commentService = commentService;
            _postService = postService;
        }

        public async Task<IQueryable<Comment>> LoadMoreCommentsHelper(int offset, int derivingFrom, int postId)
        {
            var allComments = _commentService.IQueryableGetAllAsync();

            return allComments.OrderByDescending(x => x.DateOfCreation)
                                .Where(x => x.Post.Id == postId && x.CommentDeriveFromId == derivingFrom)
                                .Skip(offset)
                                .Take(20)
                                .Include(x => x.AppUser);
        }

        public async Task<List<CardDTO>> LoadMorePostsHelper(int offset)
        {
            var allPosts = _postService.IQueryableGetAllAsync();

            var cards = allPosts.OrderByDescending(x => x.Time).Skip(offset).Take(20);

            var posts = cards.Select(post => new CardDTO
            {
                Id = post.Id,
                CreatorUserName = post.CreatorUserName,
                AmountOfComments = post.Comments.Count(),
                AmountOfLikes = post.AmountOfLikes,
                image = post.ImageLink,
                Time = post.Time
            }).ToList();

            return posts;
        }
    }
}
