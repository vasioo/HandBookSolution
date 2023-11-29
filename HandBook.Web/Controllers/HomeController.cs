
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using HandBook.Models;
using HandBook.Web.Data;
using HandBook.Web.Models;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServiceStack;
using System.Diagnostics;
using System.Configuration;
using ServiceStack.Html;
using System.Text;

namespace HandBook.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbc;
        public IConfiguration Configuration;
        private CloudinarySettings _cloudinarySettings;
        private Cloudinary _cloudinary;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbc,
            IConfiguration configuration, UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbc = dbc;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            Configuration = configuration;
            _cloudinarySettings = Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>() ?? new CloudinarySettings();
            CloudinaryDotNet.Account account = new CloudinaryDotNet.Account(
              _cloudinarySettings.CloudName,
              _cloudinarySettings.ApiKey,
              _cloudinarySettings.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);
                var cards = _dbc.Posts.OrderByDescending(x => x.Time).Include(p => p.Comments).Take(20);

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
                    var userLikedCards = _dbc.Likes.Where(like => like.UserId == user!.Id && like.CommentId == 0);
                    var userLikedComments = _dbc.Likes.Where(com => com.AppUser.Id == user!.Id && com.CommentId != 0);

                    if (userLikedCards != null && userLikedCards.Count() > 0)
                    {
                        TempData["UserLikedCards"] = userLikedCards.Select(x => x.PostId).ToList();
                    }

                    if (userLikedComments != null && userLikedComments.Count() > 0)
                    {
                        string commentIdsString = string.Join(",", userLikedComments.Select(x => x.CommentId));

                        TempData["UserLikedComments"] = commentIdsString;

                    }
                }

                ViewBag.UserUsername = username;

                return View("~/Views/Home/Index.cshtml", posts);
            }
            catch (Exception)
            {
                return View();
                throw;
            }
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View("~/Views/Home/Privacy.cshtml");
        }

        [Authorize]
        public async Task<IActionResult> DesiredPost(int modelData)
        {
            var item = await _dbc.Posts.Where(x => x.Id == modelData).FirstOrDefaultAsync();
            return View("~/Views/Home/DesiredPost.cshtml", item);
        }

        [Authorize]
        public async Task<IActionResult> Notifications()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);
            var followers = _dbc.Followers.Where(f => f.FollowedUserId == user.Id).Select(f => f.FollowerUserId);

            var cards = await _dbc.Notifications
                .Where(card => followers.Contains(card.UserId))
                .OrderBy(x => x.Time)
                .ToListAsync();

            cards.Reverse();
            ViewBag.CurrentUserUsername = user!.UserName;
            return View("~/Views/Home/Notifications.cshtml", cards);
        }

        [Authorize]
        public IActionResult AddAPost()
        {
            return View("~/Views/Home/AddAPost.cshtml");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddAPost(Post tfm, IFormFile ImageUrl, Notification ntf)
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);
            Random random = new Random();
            string randomId = GenerateRandomId(8, random);

            // Declare the transaction outside the try block
            using (var transaction = await _dbc.Database.BeginTransactionAsync())
            {
                try
                {
                    if (user != null)
                    {
                        tfm.CreatorUserName = username!;

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
                            // Database operations (e.g., SaveChangesAsync)
                            await _dbc.Posts.AddAsync(tfm);
                            await _dbc.Notifications.AddAsync(ntf);
                            await _dbc.SaveChangesAsync();

                            // Commit the transaction if everything is successful
                            await transaction.CommitAsync();

                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle the exception, log it, and/or return an appropriate response
                    // Rollback the transaction to avoid partial updates
                    await transaction.RollbackAsync();
                    return Error();
                }
            }

            return Error();
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

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AddOrRemoveAComment(CommentsDTO commentsDTO)
        {
            try
            {
                var item = await _dbc.Posts.FirstOrDefaultAsync(i => i.Id == commentsDTO.PostId);
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);

                var existingComment = await _dbc.Comments.FirstOrDefaultAsync(x => x.Id == commentsDTO.Id);

                // Add logging
                Guid randomGuid = Guid.NewGuid();
                string randomGuidString = randomGuid.ToString();

                if (existingComment != null)
                {
                    var existingNotif = await _dbc.Notifications.FirstOrDefaultAsync(x => x.UserId == user!.Id && x.PostId == commentsDTO.PostId && x.MainText == "commented on your post");

                    _dbc.Comments.Remove(existingComment);

                    if (existingNotif != null)
                    {
                        _dbc.Notifications.Remove(existingNotif);
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

                        await _dbc.AddAsync(ntf);
                    }
                    await _dbc.Comments.AddAsync(comment);
                }

                await _dbc.SaveChangesAsync();

                var neededComment = await _dbc.Comments.FirstOrDefaultAsync(x => x.UniqueIdentifier.Equals(randomGuidString));

                var commentDto = new CommentsDTO
                {
                    Id = neededComment.Id,
                    UserUsername = username,
                    CommentContent = neededComment.CommentContent,
                    CommentDeriveFromId = neededComment.CommentDeriveFromId,
                    DateOfCreation = neededComment.DateOfCreation,
                    PostId = neededComment.Post.Id,
                };

                var jsonResult = JsonConvert.SerializeObject(commentDto);

                return Json(jsonResult);
            }
            catch (Exception ex)
            {
                // Add logging
                Console.WriteLine($"Error occurred: {ex.Message}");

                return Json("Error occurred while processing the like/unlike action.");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> IncrementOrDecrementLikeCount(int itemId)
        {
            try
            {
                var item = await _dbc.Posts.FirstOrDefaultAsync(i => i.Id == itemId);
                int likeCount = item!.AmountOfLikes;
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);

                var existingLike = await _dbc.Likes.FirstOrDefaultAsync(x => x.UserId == user!.Id && x.PostId == itemId);

                if (existingLike != null)
                {
                    var existingNotif = await _dbc.Notifications.FirstOrDefaultAsync(x => x.UserId == user!.Id && x.PostId == itemId && x.MainText == "liked your post");

                    item.AmountOfLikes--;
                    _dbc.Likes.Remove(existingLike);
                    if (existingNotif != null)
                    {
                        _dbc.Notifications.Remove(existingNotif);
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
                        await _dbc.AddAsync(ntf);
                    }
                    await _dbc.Likes.AddAsync(like);
                }

                await _dbc.SaveChangesAsync();

                return Json(item.AmountOfLikes);
            }
            catch (Exception)
            {
                return Json("Error occurred while processing the like/unlike action.");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> IncrementOrDecrementCommentLikeCount(int itemId)
        {
            try
            {
                var item = await _dbc.Comments.FirstOrDefaultAsync(i => i.Id == itemId);
                int likeCount = item!.AmountOfLikes;
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);

                var existingLike = await _dbc.Likes.FirstOrDefaultAsync(x => x.AppUser.Id == user!.Id && x.CommentId == itemId);

                if (existingLike != null)
                {
                    var existingNotif = await _dbc.Notifications.FirstOrDefaultAsync(x => x.UserId == user!.Id && x.PostId == itemId && x.MainText == "liked your comment");

                    item.AmountOfLikes--;
                    _dbc.Likes.Remove(existingLike);
                    if (existingNotif != null)
                    {
                        _dbc.Notifications.Remove(existingNotif);
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
                        await _dbc.AddAsync(ntf);
                    }
                    await _dbc.Likes.AddAsync(like);
                }

                await _dbc.SaveChangesAsync();

                return Json(item.AmountOfLikes);
            }
            catch (Exception)
            {
                return Json("Error occurred while processing the like/unlike action.");
            }
        }

        [Authorize]
        public async Task<IActionResult> Account(string username)
        {
            var useraccdto = new UserAccountDTO();

            if (string.IsNullOrEmpty(username))
            {
                username = "[Error]";
                return View("~/Views/Home/Account.cshtml", useraccdto);
            }

            username = username.Trim();

            var user = await _userManager.FindByNameAsync(username);
            useraccdto.UserTempUsername = user?.UserName;

            var posts = _dbc.Posts
                .Where(x => x.CreatorUserName == username)
                .OrderByDescending(x => x.Time)
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

            useraccdto.PostsTemp = posts.ToList();
            var currusername = HttpContext.User?.Identity?.Name ?? "";
            var curruser = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                var userLikedCards = _dbc.Likes.Where(like => like.UserId == user!.Id);
                var userLikedComments = _dbc.Likes.Where(com => com.AppUser.Id == user!.Id && com.CommentId != 0);
                if (userLikedCards != null && userLikedCards.Count() > 0)
                {
                    TempData["UserLikedCards"] = userLikedCards.Select(x => x.PostId).ToList();
                }

                if (userLikedComments != null && userLikedComments.Count() > 0)
                {
                    string commentIdsString = string.Join(",", userLikedComments.Select(x => x.CommentId));

                    TempData["UserLikedComments"] = commentIdsString;
                }
            }
            bool isConnected = _dbc.Followers.Any(f => f.FollowerUserId == curruser.Id && f.FollowedUserId == user.Id);
            var countOfFollows = _dbc.Followers.Where(p => p.FollowedUserId == curruser!.Id).Count();
            var countOfFollowers = _dbc.Followers.Where(p => p.FollowerUserId == user!.Id).Count();

            ViewBag.Follows = countOfFollows;
            ViewBag.Followers = countOfFollowers;
            ViewBag.FollowsThePerson = isConnected;
            ViewBag.IsTheSamePerson = isConnected;

            return View("~/Views/Home/Account.cshtml", useraccdto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> SearchUsers()
        {
            return View("~/Views/Home/SearchUsers.cshtml");
        }

        public async Task<IActionResult> SearchUsersFilter(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var users = _userManager.Users;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(usr => usr.UserName.Contains(searchString));
            }

            var userResults = users.Select(u => new { u.UserName }).ToList(); // Adjust the properties you want to return

            return Json(userResults);
        }

        [Authorize]
        public async Task<IActionResult> AddFollowerRelationship(string username)
        {
            try
            {
                if (username == null || username == "")
                {
                    return View("~/Views/Home/Index.cshtml");
                }
                var usernamefollower = HttpContext.User?.Identity?.Name ?? "";

                var follow = await _userManager.FindByNameAsync(username);
                var follower = await _userManager.FindByNameAsync(usernamefollower);

                if (follow != null && follower != null&&usernamefollower!=username)
                {
                    var followerrelation = new Followers();

                    followerrelation.Follower = follower;
                    followerrelation.Followed = follow;

                    await _dbc.Followers.AddAsync(followerrelation);

                    Notification ntf = new Notification
                    {
                        AppUser = follow!,
                        CreatorUserName = follow!.UserName!,
                        Time = DateTime.Now,
                        MainText = "started followed you"
                    };

                    await _dbc.AddAsync(ntf);
                }
                await _dbc.SaveChangesAsync();
                return Json("Success");
            }
            catch (Exception)
            {
                return Json("Error occurred while processing the relation.");
            }
        }

        [Authorize]
        public async Task<IActionResult> RemoveFollowerRelationship(string username)
        {
            try
            {
                if (username == null || username == "")
                {
                    return View("~/Views/Home/Index.cshtml");
                }
                var usernamefollower = HttpContext.User?.Identity?.Name ?? "";

                var follow = await _userManager.FindByNameAsync(username);
                var follower = await _userManager.FindByNameAsync(usernamefollower);

                if (follow != null && follower != null && usernamefollower != username)
                {
                    var followerrelation = new Followers();

                    followerrelation.Follower = follower;
                    followerrelation.Followed = follow;

                    _dbc.Followers.Remove(followerrelation);

                    var existingNotif = await _dbc.Notifications.FirstOrDefaultAsync(x => x.UserId == follow!.Id && x.MainText == "started followed you");
                    if (existingNotif != null)
                    {
                        _dbc.Notifications.Remove(existingNotif);
                    }
                }
                await _dbc.SaveChangesAsync();
                return Json("Success");
            }
            catch (Exception)
            {
                return Json("Error occurred while processing the relation.");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> LoadMorePosts(int offset)
        {
            try
            {
                var cards = _dbc.Posts.OrderByDescending(x => x.Time).Skip(offset).Take(20);

                var posts = await cards.Select(post => new CardDTO
                {
                    Id = post.Id,
                    CreatorUserName = post.CreatorUserName,
                    AmountOfComments = post.Comments.Count(),
                    AmountOfLikes = post.AmountOfLikes,
                    image = post.ImageLink,
                    Time = post.Time
                }).ToListAsync();

                return Json(JsonConvert.SerializeObject(posts));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return Json("Error occurred while adding newer posts.");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> LoadMoreComments(int offset, int derivingFrom, int postId)
        {
            try
            {
                var comments = _dbc.Comments.OrderByDescending(x => x.DateOfCreation)
                                 .Where(x => x.Post.Id == postId && x.CommentDeriveFromId == derivingFrom)
                                 .Skip(offset)
                                 .Take(20)
                                 .Include(x => x.AppUser)
                                 .ToList();

                return Json(JsonConvert.SerializeObject(comments));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return Json("Error occurred while adding newer posts.");
            }
        }
    }
}