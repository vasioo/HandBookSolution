
using HandBook.Models;
using HandBook.Web.Data;
using HandBook.Web.Models;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace HandBook.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbc;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbc, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbc = dbc;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);
                var cards = _dbc.Posts.OrderByDescending(x => x.Time);

                if (user != null)
                {
                    var userLikedCards = _dbc.Likes.Where(like => like.UserId == user!.Id);
                    if (userLikedCards != null && userLikedCards.Count() > 0)
                    {
                        ViewBag.UserLikedCards = userLikedCards.Select(x => x.PostId).ToList();
                    }
                }

                ViewBag.UserUsername = username;

                return View("~/Views/Home/Index.cshtml", cards);
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
        public async Task<IActionResult> DesiredPost([FromBody] int modelData)
        {
            var item = await _dbc.Posts.Where(x => x.Id == modelData).FirstOrDefaultAsync();
            return View("~/Views/Home/DesiredPost.cshtml", item);
        }

        [Authorize]
        public async Task<IActionResult> Notifications()
        {
            var cards = await _dbc.Notifications.ToListAsync();
            cards.Reverse();
            return View("~/Views/Home/Notifications.cshtml", cards);
        }

        [Authorize]
        public IActionResult AddAPost()
        {
            return View("~/Views/Home/AddAPost.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddAPost(Post tfm, IFormFile image, Notification ntf)
        {
            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image!.CopyToAsync(memoryStream);
                    tfm.image = memoryStream.ToArray();
                }
            }
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                tfm.CreatorUserName = username!;
                ntf.AppUser = user!;
                ntf.CreatorUserName=user!.UserName!;
                ntf.Time = DateTime.Now;
                ntf.MainText = "added a new post";
                await _dbc.AddAsync(tfm);
                await _dbc.AddAsync(ntf);
                await _dbc.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return Error();

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddOrRemoveAComment(CommentsDTO commentsDTO )
        {
            try
            {
                var item = await _dbc.Posts.FirstOrDefaultAsync(i => i.Id == commentsDTO.PostId);
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);

                var existingComment = await _dbc.Comments.FirstOrDefaultAsync(x=>x.Id==commentsDTO.Id);
                Guid randomGuid = Guid.NewGuid();

                string randomGuidString = randomGuid.ToString();
                if (existingComment!=null)
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

                    Notification ntf = new Notification();
                    ntf.AppUser = user!;
                    ntf.CreatorUserName = user!.UserName!;
                    ntf.Post = item!;
                    ntf.Time = DateTime.Now;
                    ntf.MainText = "commented on your post";
                    await _dbc.AddAsync(ntf);
                    await _dbc.Comments.AddAsync(comment);
                }

                await _dbc.SaveChangesAsync();

                var neededComment = await _dbc.Comments.FirstOrDefaultAsync(x => x.UniqueIdentifier== randomGuidString);

                return Json(neededComment);
            }
            catch (Exception)
            {
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

                    Notification ntf = new Notification();
                    ntf.AppUser = user!;
                    ntf.CreatorUserName = user!.UserName!;
                    ntf.Post = item!;
                    ntf.Time = DateTime.Now;
                    ntf.MainText = "liked your post";
                    await _dbc.AddAsync(ntf);
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
            if (username==null||username=="")
            {
                return View("~/Views/Home/Index.cshtml");
            }
            var user = await _userManager.FindByNameAsync(username);

            var useraccdto = new UserAccountDTO();
            useraccdto.UserTemp = user;
            useraccdto.PostsTemp = _dbc.Posts.Where(x => x.CreatorUserName == username);

            if (user != null)
            {
                var userLikedCards = _dbc.Likes.Where(like => like.UserId == user!.Id);
                if (userLikedCards != null && userLikedCards.Count() > 0)
                {
                    ViewBag.UserLikedCards = userLikedCards.Select(x => x.PostId).ToList();
                }
            }

            ViewBag.UserUsername = username;


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
    }
}