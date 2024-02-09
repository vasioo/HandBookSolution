using HandBook.Models;
using HandBook.Web.Models;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceStack;
using System.Diagnostics;

namespace HandBook.Web.Controllers.HomeControllerFolder
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public IHCHelper _helper { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor, IHCHelper helper)
        {
            _logger = logger;
            _helper = helper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);
                ViewBag.UserUsername = username;
                var posts = _helper.IndexHelper(user!);

                if (posts.Count() > 0)
                {
                    TempData["UserLikedCards"] = posts.First().UserLikedCards;
                    TempData["UserLikedComments"] = posts.First().UserLikedComments;
                }

                return View("~/Views/Home/Index.cshtml", posts);
            }
            catch (Exception)
            {
                return View();
                throw;
            }
        }

        public async Task<IActionResult> DesiredPost(Guid postId)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);

                var item = await _helper.DesiredPostHelper(postId, user!);

                if (item!=null)
                {
                    TempData["UserLikedComments"] = item.UserLikedComments;
                }

                return View("~/Views/Home/DesiredPost.cshtml", item);
            }
            catch (Exception)
            {
                return View();
                throw;
            }
            
        }

        public async Task<IActionResult> Notifications()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var notifications = await _helper.NotificationsHelper(user!);
            ViewBag.CurrentUserUsername = user!.UserName;

            return View("~/Views/Home/Notifications.cshtml", notifications);
        }

        public IActionResult AddAPost()
        {
            return View("~/Views/Home/AddAPost.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAPost(Post tfm, IFormFile ImageUrl, Notification ntf)
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            try
            {
                await _helper.AddAPostHelper(tfm, ImageUrl, ntf, user);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return Error();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AddOrRemoveAComment(CommentsDTO commentsDTO)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);
                var jsonResult = await _helper.AddOrRemoveACommentHelper(commentsDTO, user);
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
        public async Task<IActionResult> IncrementOrDecrementLikeCount(Guid itemId)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);

                var item = await _helper.IncrementOrDecrementLikeCountHelper(itemId, user);

                return Json(item);
            }
            catch (Exception)
            {
                return Json("Error occurred while processing the like/unlike action.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> IncrementOrDecrementCommentLikeCount(Guid itemId)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);

                var item = _helper.IncrementOrDecrementCommentLikeCountHelper(itemId, user!);

                return Json(item);
            }
            catch (Exception)
            {
                return Json("Error occurred while processing the like/unlike action.");
            }
        }

        public async Task<IActionResult> Account(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                username = "[Error]";
                return View("~/Views/Home/Account.cshtml", new UserAccountDTO());
            }

            username = username.Trim();

            var user = await _userManager.FindByNameAsync(username);
            var currusername = HttpContext.User?.Identity?.Name ?? "";
            var curruser = await _userManager.FindByNameAsync(currusername);

            var useraccdto = await _helper.AccountHelper(user!, curruser!);

            return View("~/Views/Home/Account.cshtml", useraccdto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SearchUsers()
        {
            return View("~/Views/Home/SearchUsers.cshtml");
        }

        public IActionResult SearchUsersFilter(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var users = _userManager.Users;

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(usr => usr.UserName.Contains(searchString));
            }

            var userResults = users.Select(u => new { u.UserName }).ToList(); // Adjust the properties you want to return

            return Json(userResults);
        }

        public async Task<IActionResult> AddFollowerRelationship(string username)
        {
            try
            {
                if (username == null || username == "")
                {
                    return View("~/Views/Home/Index.cshtml");
                }
                var usernamefollower = HttpContext.User?.Identity?.Name ?? "";

                await _helper.AddFollowerRelationshipHelper(username, usernamefollower);
                return Json("Success");
            }
            catch (Exception)
            {
                return Json("Error occurred while processing the relation.");
            }
        }

        public async Task<IActionResult> RemoveFollowerRelationship(string username)
        {
            try
            {
                if (username == null || username == "")
                {
                    return View("~/Views/Home/Index.cshtml");
                }
                var usernamefollower = HttpContext.User?.Identity?.Name ?? "";

                await _helper.RemoveFollowerRelationshipHelper(username, usernamefollower);
                return Json("Success");
            }
            catch (Exception)
            {
                return Json("Error occurred while processing the relation.");
            }
        }

        [HttpPost]
        public async Task<JsonResult> LoadMorePosts(int offset)
        {
            try
            {
                return Json(JsonConvert.SerializeObject(await _helper.LoadMorePostsHelper(offset)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return Json("Error occurred while adding newer posts.");
            }
        }

        [HttpPost]
        public JsonResult LoadMoreComments(int offset, Guid derivingFrom, Guid postId)
        {
            try
            {
                return Json(JsonConvert.SerializeObject(_helper.LoadMoreCommentsHelper(offset, derivingFrom, postId)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return Json("Error occurred while adding newer posts.");
            }
        }

        public IActionResult Privacy()
        {
            return View("~/Views/Home/Privacy.cshtml");
        }

        public async Task<IActionResult> ExplorePage()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel =await _helper.GetExplorePageAttributes(user!);

            return View("~/Views/Home/ExplorePage.cshtml",viewModel);
        }
    }
}