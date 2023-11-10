
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
                var user = await _userManager.GetUserAsync(User);
                var cards = _dbc.Posts.OrderByDescending(x => x.Time);

                var userLikedCards = _dbc.Likes.Where(like => like.UserId == user!.Id);

                ViewBag.UserLikedCards = userLikedCards.Select(x => x.PostId).ToList();

                return View(cards);
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
            return View();
        }

        [Authorize]
        public async Task<IActionResult> DesiredPost([FromBody] int modelData)
        {
            var item = await _dbc.Posts.Where(x => x.Id == modelData).FirstOrDefaultAsync();
            return View(item);
        }
        
        [Authorize]
        public async Task<IActionResult> Notifications()
        {
            var cards = await _dbc.Notifications.ToListAsync();
            cards.Reverse();
            return View(cards);
        }

        [Authorize]
        public IActionResult AddAPost()
        {
            return View();
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
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var item = _dbc.Posts.OrderBy(x => x.Id).Last();
                tfm.CreatorUserName = user!.UserName!;
                ntf.CreatorUserName = user!.UserName!;
                ntf.PostId = item.Id;
                ntf.CreatorUserName = tfm!.CreatorUserName!;
                ntf.Time = DateTime.Now;
                ntf.MainText = "added a new post";
                await _dbc.AddAsync(ntf);
                await _dbc.AddAsync(tfm);
                await _dbc.SaveChangesAsync();

                Thread.Sleep(1000);
                return RedirectToAction("Index");
            }
            return Error();

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> IncrementOrDecrementLikeCount(int itemId)
        {
            try
            {
                var item = await _dbc.Posts.FirstOrDefaultAsync(i => i.Id == itemId);
                int likeCount = item!.AmountOfLikes;
                var user = await _userManager.GetUserAsync(User);

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
                        PostId = item.Id,
                        UserId = user!.Id,
                        LikedDate = DateTime.Now
                    };

                    Notification ntf = new Notification();
                    ntf.CreatorUserName = user!.UserName!;
                    ntf.PostId = item.Id;
                    ntf.Time = DateTime.Now;
                    ntf.MainText = "liked your post";
                    ntf.UserId = user!.Id;
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}