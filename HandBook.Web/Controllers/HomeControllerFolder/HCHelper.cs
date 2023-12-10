using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using HandBook.Models;
using HandBook.Services.Interfaces;
using HandBook.Web.Models;
using Messenger.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text;
using HandBook.DataAccess;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Data.Entity;

namespace HandBook.Web.Controllers.HomeControllerFolder
{
    public class HCHelper : IHCHelper
    {
        public readonly UserManager<AppUser> _userManager;

        private readonly ICommentService _commentService;
        private readonly INotificationService _notificationService;
        private readonly IPostService _postService;
        private readonly ILikesService _likeService;
        private readonly IFollowerService _followerService;
        private CloudinarySettings _cloudinarySettings;
        public IConfiguration Configuration;
        private Cloudinary _cloudinary;

        public HCHelper(UserManager<AppUser> userManager, IConfiguration configuration, IFollowerService followerService, ILikesService likeService, ICommentService commentService, IPostService postService, INotificationService notificationService)
        {
            _userManager = userManager;
            _commentService = commentService;
            _postService = postService;
            _notificationService = notificationService;
            Configuration = configuration;
            _cloudinarySettings = Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>() ?? new CloudinarySettings();
            Account account = new Account(
              _cloudinarySettings.CloudName,
              _cloudinarySettings.ApiKey,
              _cloudinarySettings.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _likeService = likeService;
            _followerService = followerService;
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

        private async Task<string> UploadToCloudinaryAsync(IFormFile file, string imageName, string publicId)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(imageName, stream),
                    DisplayName = imageName,
                    PublicId = publicId,
                    Overwrite = false,
                    // You can add more parameters as needed
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.AbsoluteUri;
            }
        }

        static string GenerateRandomId(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder idBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                idBuilder.Append(chars[index]);
            }

            return idBuilder.ToString();
        }

        public async Task AddAPostHelper(Post tfm, IFormFile ImageUrl, Notification ntf, AppUser user)
        {
            Random random = new Random();
            string randomId = GenerateRandomId(8, random);
            if (user != null)
            {
                tfm.CreatorUserName = user.UserName!;

                ntf.Post = tfm;
                ntf.AppUser = user!;
                ntf.CreatorUserName = user!.UserName!;
                ntf.Time = DateTime.Now;
                ntf.MainText = "added a new post";

                if (ImageUrl != null && ImageUrl.Length > 0)
                {
                    tfm.ImageLink = await UploadToCloudinaryAsync(ImageUrl, $"post-{randomId}", $"post-{randomId}");
                }

                if (tfm != null)
                {
                    await _postService.AddAsync(tfm);
                    await _notificationService.AddAsync(ntf);
                }
            }
        }

        public async Task<string> AddOrRemoveACommentHelper(CommentsDTO commentsDTO, AppUser user)
        {
            var item = await _postService.GetByIdAsync(commentsDTO.PostId);


            var existingComment = await _commentService.GetByIdAsync(commentsDTO.Id);

            // Add logging
            Guid randomGuid = Guid.NewGuid();
            string randomGuidString = randomGuid.ToString();

            if (existingComment != null)
            {
                var existingNotif = await _notificationService.GetExistingNotification(user.Id, commentsDTO.PostId, "commented on your post");

                await _commentService.RemoveAsync(existingComment.Id);

                if (existingNotif != null)
                {
                    await _notificationService.RemoveAsync(existingNotif!.Id);
                }
            }
            else
            {
                var comment = new Comment
                {
                    DateOfCreation = DateTime.Now,
                    Post = item,
                    AppUser = user,
                    CommentDeriveFromId = commentsDTO.CommentDeriveFromId,
                    CommentContent = commentsDTO.CommentContent,
                    UniqueIdentifier = randomGuidString
                };
                if (item!.CreatorUserName != user!.UserName)
                {
                    Notification ntf = new Notification
                    {
                        AppUser = user!,
                        CreatorUserName = user!.UserName!,
                        Post = item!,
                        Time = DateTime.Now,
                        MainText = "commented on your post"
                    };

                    await _notificationService.AddAsync(ntf);
                }
                await _commentService.AddAsync(comment);
            }

            var neededComment = _commentService.GetCommentBasedOnRandomGuid(randomGuidString);

            var commentDto = new CommentsDTO
            {
                Id = neededComment!.Id,
                UserUsername = user!.UserName!,
                CommentContent = neededComment.CommentContent,
                CommentDeriveFromId = neededComment.CommentDeriveFromId,
                DateOfCreation = neededComment.DateOfCreation,
                PostId = neededComment.Post.Id,
            };

            var jsonResult = JsonConvert.SerializeObject(commentDto);

            return jsonResult;
        }

        public async Task<int> IncrementOrDecrementLikeCountHelper(int itemId, AppUser user)
        {
            var item = await _postService.GetByIdAsync(itemId);

            var existingLike = _likeService.GetLikeEntityForUserAndPostInfo(user.Id, itemId);

            if (existingLike != null)
            {
                var existingNotif = await _notificationService.GetExistingNotification(user.Id, itemId, "liked your post");

                item.AmountOfLikes--;
                await _likeService.RemoveAsync(existingLike!.Id);
                if (existingNotif != null)
                {
                    await _notificationService.RemoveAsync(existingNotif!.Id);
                }
            }
            else
            {
                item.AmountOfLikes++;
                var like = new Likes
                {
                    Post = item,
                    AppUser = user!,
                    LikedDate = DateTime.Now
                };

                if (item.CreatorUserName != user.UserName)
                {
                    Notification ntf = new Notification();
                    ntf.AppUser = user!;
                    ntf.CreatorUserName = user!.UserName!;
                    ntf.Post = item!;
                    ntf.Time = DateTime.Now;
                    ntf.MainText = "liked your post";
                    await _notificationService.AddAsync(ntf);
                }
                await _likeService.AddAsync(like);
            }
            return item.AmountOfLikes;
        }

        public async Task<int> IncrementOrDecrementCommentLikeCountHelper(int itemId, AppUser user)
        {
            var item = await _commentService.GetByIdAsync(itemId);

            var existingLike = _likeService.GetLikeEntityForUserAndCommentInfo(user.Id, itemId);

            if (existingLike != null)
            {
                var existingNotif = await _notificationService.GetExistingNotification(user.Id, itemId, "liked your comment");
                item.AmountOfLikes--;
                await _likeService.RemoveAsync(existingLike!.Id);
                if (existingNotif != null)
                {
                    await _notificationService.RemoveAsync(existingNotif!.Id);
                }
            }
            else
            {
                item.AmountOfLikes++;
                var like = new Likes
                {
                    Post = item.Post,
                    AppUser = user!,
                    LikedDate = DateTime.Now,
                    CommentId = item.Id
                };

                if (item.AppUser.UserName != user.UserName)
                {
                    Notification ntf = new Notification();
                    ntf.AppUser = user!;
                    ntf.CreatorUserName = user!.UserName!;
                    ntf.Post = item!.Post;
                    ntf.Time = DateTime.Now;
                    ntf.MainText = "liked your comment";
                    await _notificationService.AddAsync(ntf);
                }
                await _likeService.AddAsync(like);
            }

            return item.AmountOfLikes;
        }

        public async Task<UserAccountDTO> AccountHelper(AppUser user, AppUser currUser)
        {
            var useraccdto = new UserAccountDTO();
            useraccdto.UserTempUsername = user?.UserName;


            useraccdto.PostsTemp = _postService.GetPostsBasedOnCreatorUser(user!.UserName!);


            if (user != null)
            {
                var userLikedCards = await _likeService.GetUserLikedPosts(user.Id);
                var userLikedComments = await _likeService.GetUserLikedPosts(user.Id);
                if (userLikedCards != null && userLikedCards.Count() > 0)
                {
                    useraccdto.UserLikedCards = userLikedCards.Select(x => x.PostId).ToList();
                }

                if (userLikedComments != null && userLikedComments.Count() > 0)
                {
                    string commentIdsString = string.Join(",", userLikedComments.Select(x => x.CommentId));

                    useraccdto.UserLikedComments = commentIdsString;
                }
            }
            bool isConnected = await _followerService.FindIfUserIsFollowed(user!.Id, currUser.Id);
            var countOfFollows = await _followerService.GetFollowedCount(currUser.Id);
            var countOfFollowers = await _followerService.GetFollowerCount(user!.Id);

            useraccdto.Follows = countOfFollows;
            useraccdto.Followers = countOfFollowers;
            useraccdto.FollowsThePerson = isConnected;
            useraccdto.IsTheSamePerson = isConnected;

            return useraccdto;
        }

        public async Task AddFollowerRelationshipHelper(string username, string usernamefollower)
        {
            var follow = await _userManager.FindByNameAsync(username);
            var follower = await _userManager.FindByNameAsync(usernamefollower);

            if (follow != null && follower != null && usernamefollower != username)
            {
                var followerrelation = new Followers();

                followerrelation.Follower = follower;
                followerrelation.Followed = follow;

                await _followerService.AddAsync(followerrelation);

                Notification ntf = new Notification
                {
                    AppUser = follow!,
                    CreatorUserName = follow!.UserName!,
                    Time = DateTime.Now,
                    MainText = "started followed you"
                };

                await _notificationService.AddAsync(ntf);
            }
        }

        public async Task RemoveFollowerRelationshipHelper(string username, string usernamefollower)
        {
            var follow = await _userManager.FindByNameAsync(username);
            var follower = await _userManager.FindByNameAsync(usernamefollower);

            if (follow != null && follower != null && usernamefollower != username)
            {
                var followerrelation = new Followers();

                followerrelation.Follower = follower;
                followerrelation.Followed = follow;

                await _followerService.RemoveAsync(followerrelation.Id);

                var existingNotif = await _notificationService.GetExistingNotification(follow.Id,0, "started followed you");
                if (existingNotif != null)
                {
                    await _notificationService.RemoveAsync(existingNotif!.Id);
                }
            }
        }

        public async Task<IOrderedQueryable<Notification>> NotificationsHelper(AppUser user)
        {
            var cards = await _notificationService.GetNotificationsByUserId(user.Id);

            return cards;
        }

        public async Task<Post> DesiredPostHelper(int desiredPostId)
        {
            return await _postService.GetByIdAsync(desiredPostId);
        }

        public async Task<List<CardDTO>> IndexHelper(AppUser user)
        {
            var cards = _postService.IQueryableGetAllAsync().OrderByDescending(x => x.Time).Include(p => p.Comments).Take(20);

            var posts = cards.Select(post => new CardDTO
            {
                Id = post.Id,
                CreatorUserName = post.CreatorUserName,
                AmountOfComments = post.Comments.Count(),
                AmountOfLikes = post.AmountOfLikes,
                Time = post.Time,
                image = post.ImageLink
            }).ToList();


            if (user != null)
            {
                var userLikedCards = await _likeService.GetUserLikedPosts(user.Id);
                var userLikedComments = await _likeService.GetUserLikedComments(user.Id);

                if (userLikedCards != null && userLikedCards.Count() > 0)
                {
                    posts[0].UserLikedCards = userLikedCards.Select(x => x.PostId).ToList();
                }

                if (userLikedComments != null && userLikedComments.Count() > 0)
                {
                    string commentIdsString = string.Join(",", userLikedComments.Select(x => x.CommentId));

                    posts[0].UserLikedComments = commentIdsString;

                }
            }

            return posts;
        }
    }
}
