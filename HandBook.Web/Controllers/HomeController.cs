using HandBook.Data;
using HandBook.Models;
using HandBook.Web.Data;
using HandBook.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HandBook.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbc;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbc, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbc = dbc;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);
                var cards = _dbc.Posts.OrderByDescending(x=>x.Time);

                var userLikedCards = _dbc.Likes.Where(like => like.UserId ==user.Id);

                ViewBag.UserLikedCards = userLikedCards.Select(x=>x.PostId);

                return View(cards);
            }
            catch (Exception)
            {
                return View();
                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult DesiredPost([FromBody] int modelData)
        {
            Post item = _dbc.Posts.Where(x => x.Id == modelData).FirstOrDefault();
            return View(item);
        }

        public IActionResult Notifications()
        {
            var cards = _dbc.Notifications.ToList();
            cards.Reverse();
            return View(cards);
        }

        public IActionResult AddAPost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAPost(Post tfm, IFormFile image, Notification ntf)
        {
            if (image != null && image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    image.CopyToAsync(memoryStream);
                    tfm.image = memoryStream.ToArray();
                }
            }

            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                tfm.CreatorUserName = user.UserName;

                _dbc.Add(tfm);
                _dbc.SaveChanges();

                ntf.CreatorUserName = user.UserName;
                var item = _dbc.Posts.OrderBy(x => x.Id).Last();
                ntf.PostId = item.Id;
                ntf.CreatorUserName = tfm.CreatorUserName;
                ntf.Time = DateTime.Now;
                _dbc.Add(ntf);
                _dbc.SaveChanges();

                Thread.Sleep(1000);
                return RedirectToAction("Index");
            }
            return Error();

        }

        [HttpPost]
        public async Task<IActionResult> IncrementLikeCount(int itemId)
        {
            try
            {
                var item = _dbc.Posts.FirstOrDefault(i => i.Id == itemId);
                int likeCount = item.AmountOfLikes;
                var username = HttpContext.User?.Identity?.Name ?? "";
                var user = await _userManager.FindByNameAsync(username);

                item.AmountOfLikes++;

                var like = new Likes()
                {
                    PostId = item.Id,
                    UserId = user.Id,
                    LikedDate = DateTime.Now
                };

                _dbc.Add(like);

                await _dbc.SaveChangesAsync();

                return Json(item.AmountOfLikes);
            }
            catch (Exception)
            {
                return Json("srg");
                throw;
            }
            // Get the current like count
            
        }

        [HttpPost]
        public async Task<IActionResult> DecrementLikeCount(int itemId)
        {
            var item = _dbc.Posts.FirstOrDefault(i => i.Id == itemId);
            int likeCount = item.AmountOfLikes;
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);
            item.AmountOfLikes--;

            var like = new Likes()
            {
                PostId = item.Id,
                UserId = user.Id,
            };

            _dbc.Likes.Remove(like);

            await _dbc.SaveChangesAsync();

            return Json(item.AmountOfLikes);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}