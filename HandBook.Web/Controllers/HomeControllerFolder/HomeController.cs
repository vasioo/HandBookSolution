using HandBook.Models;
using HandBook.Models.BaseModels.ViewRender;
using HandBook.Web.Models;
using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ServiceStack;
using System.Diagnostics;
using System.Text;

namespace HandBook.Web.Controllers.HomeControllerFolder
{
    [Authorize]
    public class HomeController : Controller
    {
        #region FieldsAndConstructor
        private readonly UserManager<AppUser> _userManager;
        public IHCHelper _helper { get; set; }
        public ViewRenderer _viewRenderer { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor, IHCHelper helper, ViewRenderer viewRenderer)
        {
            _logger = logger;
            _helper = helper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _viewRenderer = viewRenderer;
        }
        #endregion

        #region Feed

        public async Task<IActionResult> Feed()
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);
                ViewBag.UserUsername = username;
                var posts = _helper.FeedHelper(user!);

                if (posts.Count() > 0)
                {
                    TempData["UserLikedCards"] = posts.First().UserLikedCards;
                    TempData["UserLikedComments"] = posts.First().UserLikedComments;
                }

                return View("~/Views/Home/Feed.cshtml", posts);
            }
            catch (Exception)
            {
                return View();
                throw;
            }
        }

        #endregion

        #region Notifications
        public async Task<IActionResult> Notifications()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var notifications = await _helper.NotificationsHelper(user!);
            ViewBag.CurrentUserUsername = user!.UserName;

            return View("~/Views/Home/Notifications.cshtml", notifications);
        }
        #endregion

        #region DesiredPost
        public async Task<IActionResult> DesiredPost(Guid postId)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);

                var item = await _helper.DesiredPostHelper(postId, user!);

                if (item != null)
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

        #endregion

        #region AddAPost
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

        public IActionResult AddAPost()
        {
            return View("~/Views/Home/AddAPost.cshtml");
        }
        #endregion

        #region HelperMethods

        #region Comments

        [HttpPost]
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
        public JsonResult LoadMoreComments(int offset, Guid derivingFrom, Guid postId)
        {
            try
            {
                var result = _helper.LoadMoreCommentsHelper(offset, derivingFrom, postId);
                return Json(new { status = true, Message = result });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return Json("Error occurred while retrieving newer posts.");
            }
        }
        #endregion

        #region Likes
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

                var item = await _helper.IncrementOrDecrementCommentLikeCountHelper(itemId, user!);

                return Json(item);
            }
            catch (Exception)
            {
                return Json("Error occurred while processing the like/unlike action.");
            }
        }

        #endregion

        #region Followers
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

        public async Task<IActionResult> RemoveFollowerRelationship(string username, bool shouldBeReversed = false)
        {
            try
            {
                if (username == null || username == "")
                {
                    return View("~/Views/Home/Index.cshtml");
                }
                var usernamefollower = HttpContext.User?.Identity?.Name ?? "";

                if (shouldBeReversed)
                {
                    await _helper.RemoveFollowerRelationshipHelper(usernamefollower, username);
                }
                else
                {
                    await _helper.RemoveFollowerRelationshipHelper(username, usernamefollower);
                }
                return Json("Success");
            }
            catch (Exception)
            {
                return Json("Error occurred while processing the relation.");
            }
        }

        #endregion

        #region Posts
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

        #endregion

        #region Others
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

        #endregion

        #region Account

        public async Task<IActionResult> Account(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return View("~/Views/Home/Account.cshtml", new UserAccountDTO());
            }

            username = username.Trim();

            var user = await _userManager.FindByNameAsync(username);
            var currusername = HttpContext.User?.Identity?.Name ?? "";
            var curruser = await _userManager.FindByNameAsync(currusername);

            var useraccdto = await _helper.AccountHelper(user!, curruser!);

                TempData["UserLikedCards"] = useraccdto.UserLikedCards;
                TempData["UserLikedComments"] = useraccdto.UserLikedComments;

            return View("~/Views/Home/Account.cshtml", useraccdto);
        }

        [HttpPost]
        public async Task<JsonResult> LoadFollows(bool followers, int offset)
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);

                var result = _helper.LoadFollowsBasedOnOffset(user!, followers, offset);
                return Json(new { status = true, Message = result });
            }
            catch (Exception)
            {
                return Json("Error occurred while extracting followers/followings.");
                throw;
            }
        }
        #endregion

        #region ExplorePage

        public async Task<IActionResult> ExplorePage()
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            var viewModel = await _helper.GetExplorePageAttributes(user!);

            return View("~/Views/Home/ExplorePage.cshtml", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecificExplorePageItems(Guid itemId)
        {
            try
            {
                var itemsQuery = _helper.GetSpecificExplorePageItemsByProvidedItemHelper(itemId);

                StringBuilder responseBuilder = new StringBuilder();

                foreach (var item in itemsQuery)
                {
                    var htmlContent = await _viewRenderer.RenderViewToStringAsync("_PostsPartial.cshtml", item, new[] { "Views/Home" }, HttpContext);
                    responseBuilder.AppendLine(htmlContent);
                }

                return Content(responseBuilder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return BadRequest("Error occurred while getting the posts.");
            }
        }

        #region SearchUsers
        public IActionResult SearchUsersFilter(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var users = _userManager.Users;

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(usr => usr.UserName!.Length >= searchString.Length &&
                                    usr.UserName.Substring(0, searchString.Length).Equals(searchString));
            }

            var userResults = users.Select(u => new { u.UserName }).ToList();

            return Json(userResults);
        }

        #endregion

        #endregion
    }
}