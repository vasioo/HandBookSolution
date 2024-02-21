using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HandBook.Models;
using HandBook.Models.JSONModel;
using HandBook.Models.ViewModels;
using HandBook.Services.Interfaces;
using HandBook.Web.Models;
using Messenger.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using ServiceStack;
using System.Data.Entity;
using System.Text;
using System.Xml.Linq;

namespace HandBook.Web.Controllers.HomeControllerFolder
{
    public class HCHelper : IHCHelper
    {

        #region FieldsAndConstructor
        public readonly UserManager<AppUser> _userManager;
        private readonly ICommentService _commentService;
        private readonly INotificationService _notificationService;
        private readonly IPostService _postService;
        private readonly ILikesService _likeService;
        private readonly IFollowerService _followerService;
        private CloudinarySettings _cloudinarySettings;
        public IConfiguration Configuration;
        private Cloudinary _cloudinary;

        public HCHelper(UserManager<AppUser> userManager, IConfiguration configuration,
            IFollowerService followerService, ILikesService likeService,
            ICommentService commentService, IPostService postService,
            INotificationService notificationService)
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
        #endregion

        #region FeedHelper

        public IQueryable<CardDTO> FeedHelper(AppUser user)
        {
            try
            {
                var cards =  _postService.IQueryableGetAllAsync().OrderByDescending(x => x.Time).Take(20).ToList();
                var comments =  _commentService.IQueryableGetAllAsync().ToList();

                var posts = cards.Select(post => new CardDTO
                {
                    Id = post.Id,
                    CreatorUserName = post.CreatorUserName,
                    AmountOfComments = comments.Count(x => x.Post.Id == post.Id),
                    AmountOfLikes = post.AmountOfLikes,
                    Time = post.Time,
                    image = post.ImageLink,
                    Description = post.Description
                }).ToList();

                if (user != null)
                {
                    var userLikedCards = _likeService.GetUserLikedPosts(user.Id);
                    var userLikedComments = _likeService.GetUserLikedComments(user.Id);

                    if (userLikedCards != null && userLikedCards.Any())
                    {
                        var items = userLikedCards.Select(x => x.PostId.ToString().ToLower()).ToList();
                        posts.First().UserLikedCards = items;
                    }

                    if (userLikedComments != null && userLikedComments.Any())
                    {
                        string commentIdsString = string.Join(",", userLikedComments.Select(x => x.CommentId.ToString().ToLower()));
                        posts.First().UserLikedComments = commentIdsString;
                    }
                }

                return posts.AsQueryable();
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region NotificationsHelper 

        public async Task<IOrderedQueryable<Notification>> NotificationsHelper(AppUser user)
        {
            var cards = await _notificationService.GetNotificationsByUserId(user.Id);

            return cards;
        }

        #endregion

        #region  DesiredPostHelper

        public async Task<CardDTO> DesiredPostHelper(Guid desiredPostId, AppUser user)
        {
            var post = await _postService.GetByIdAsync(desiredPostId);
            var comments = _commentService.IQueryableGetAllAsync();

            var cardDto = new CardDTO();

            cardDto.Id = post.Id;
            cardDto.CreatorUserName = post.CreatorUserName;
            cardDto.AmountOfLikes = post.AmountOfLikes;
            cardDto.image = post.ImageLink;
            cardDto.Time = post.Time;
            cardDto.AmountOfComments = comments.Where(x => x.Post.Id == post.Id).Count();
            cardDto.Description = post.Description;

            if (user != null)
            {
                var userLikedCards = _likeService.GetUserLikedPosts(user.Id);
                var userLikedComments = _likeService.GetUserLikedComments(user.Id);

                if (userLikedCards != null && userLikedCards.Count() > 0)
                {
                    cardDto.IsCurrentPostLiked = userLikedCards.Where(x => x.Id == cardDto.Id).Any();
                }

                if (userLikedComments != null && userLikedComments.Count() > 0)
                {
                    string commentIdsString = string.Join(",", userLikedComments.Where(x => x.Post.Id == cardDto.Id).Select(x => x.CommentId));

                    cardDto.UserLikedComments = commentIdsString;

                }
            }
            return cardDto;
        }

        #endregion

        #region  AddAPostHelper

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


        #endregion

        #region  HelperMethodsHelper

            #region CommentsHelper

            public async Task<string> AddOrRemoveACommentHelper(CommentsDTO commentsDTO, AppUser user)
        {
            var item = await _postService.GetByIdAsync(commentsDTO.PostId);

            var existingComment = await _commentService.GetByIdAsync(commentsDTO.Id);

            Guid randomGuid = Guid.NewGuid();

            if (existingComment.Id.ToString() != "00000000-0000-0000-0000-000000000000")
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

            var neededComment = await _commentService.GetByIdAsync(randomGuid);

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

            public IQueryable<CommentJSON> LoadMoreCommentsHelper(int offset, Guid derivingFrom, Guid postId)
            {
                var allComments = _commentService.IQueryableGetAllAsync();

                var neededComs = allComments.OrderByDescending(x => x.DateOfCreation)
                            .Where(x => x.Post.Id == postId && x.CommentDeriveFromId == derivingFrom)
                            .Skip(offset)
                            .Take(20);

                var commentsJSON = neededComs.Select(com => new CommentJSON
                {
                    Id = com.Id,
                    DateOfCreation = com.DateOfCreation,
                    CommentContent = com.CommentContent,
                    AmountOfLikes = com.AmountOfLikes,
                    AppUser = com.AppUser,
                    Post = com.Post,
                    CommentDeriveFromId = com.CommentDeriveFromId,
                    AmountOfReplies = allComments.Where(x => x.CommentDeriveFromId == com.Id).Count()
                });

                return commentsJSON;
            }


            #endregion

            #region LikesHelper

            public async Task<int> IncrementOrDecrementLikeCountHelper(Guid itemId, AppUser user)
        {
            var item = await _postService.GetByIdAsync(itemId);
            item.AmountOfLikes = _likeService.IQueryableGetAllAsync().Where(x => x.PostId == item.Id).Count();
            if (item.AmountOfLikes == -1)
            {
                item.AmountOfLikes++;
            }
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

            public async Task<int> IncrementOrDecrementCommentLikeCountHelper(Guid itemId, AppUser user)
        {
            var item = await _commentService.GetByIdAsync(itemId);
            item.AmountOfLikes = _likeService.IQueryableGetAllAsync().Where(x => x.PostId == item.Id).Count();
            if (item.AmountOfLikes == -1)
            {
                item.AmountOfLikes++;
            }

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

            #endregion

            #region FollowersHelper

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
                    var followerToRemove = _followerService.IQueryableGetAllAsync().Where(x => x.Followed.Id == follow.Id && x.Follower.Id == follower.Id).FirstOrDefault();
                    
                    await _followerService.RemoveAsync(followerToRemove.Id);

                    var tempGuid = Guid.Empty;
                    var existingNotif = await _notificationService.GetExistingNotification(follow.Id, tempGuid, "started followed you");
                    if (existingNotif != null)
                    {
                        await _notificationService.RemoveAsync(existingNotif!.Id);
                    }
                }
            }

            #endregion

            #region PostsHelper

        public async Task<List<CardDTO>> LoadMorePostsHelper(int offset)
        {
            var allPosts = _postService.IQueryableGetAllAsync();

            var cards = allPosts.OrderByDescending(x => x.Time).Skip(offset).Take(20);
            var comments = _commentService.IQueryableGetAllAsync();
            var posts = cards.Select(post => new CardDTO
            {
                Id = post.Id,
                CreatorUserName = post.CreatorUserName,
                AmountOfComments = comments.Where(x => x.Post.Id == post.Id).Count(),
                AmountOfLikes = post.AmountOfLikes,
                image = post.ImageLink,
                Time = post.Time
            }).ToList();

            return posts;
        }

        #endregion

            #region Others

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

        #endregion

        #endregion

        #region  Account

        public async Task<UserAccountDTO> AccountHelper(AppUser user, AppUser currUser)
        {
            var useraccdto = new UserAccountDTO();
            useraccdto.UserTempUsername = user?.UserName;


            useraccdto.PostsTemp = _postService.GetPostsBasedOnCreatorUser(user!.UserName!);


            if (user != null)
            {
                var userLikedCards = _likeService.GetUserLikedPosts(user.Id);
                var userLikedComments = _likeService.GetUserLikedPosts(user.Id);
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

        public IQueryable<string?> LoadFollowsBasedOnOffset(AppUser currUser,bool isFollower, int offset)
        {
            if (isFollower)
            {
                return _followerService.IQueryableGetAllAsync()
                    .Where(x => x.Followed.Id == currUser.Id)
                    .Select(x => x.Follower.UserName)
                    .Skip(offset*30);
            }
            else
            {
                return _followerService.IQueryableGetAllAsync()
                    .Where(x => x.Follower.Id== currUser.Id)
                    .Select(x => x.Followed.UserName)
                    .Skip(offset * 30);
            }
        }

        #endregion

        #region  ExlporePageHelper

        public async Task<ExplorePageViewModel> GetExplorePageAttributes(AppUser user)
        {
            var viewModel = new ExplorePageViewModel();

            viewModel.Posts = _postService.GetPostsBasedOnUserFavouritism(user);

            return viewModel;
        }

        public IQueryable<CardDTO> GetSpecificExplorePageItemsByProvidedItemHelper(Guid itemId)
        {
            var items = _postService.IQueryableGetAllAsync();
            var comments = _commentService.IQueryableGetAllAsync();
            var posts = items.Select(post => new CardDTO
            {
                Id = post.Id,
                CreatorUserName = post.CreatorUserName,
                AmountOfComments = comments.Where(x => x.Post.Id == post.Id).Count(),
                AmountOfLikes = post.AmountOfLikes,
                Time = post.Time,
                image = post.ImageLink,
                Description = post.Description
            });

            return posts;
        }

       

        #endregion

    }
}
