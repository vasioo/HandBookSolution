﻿using Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HandBook.Web.Controllers.MessagesControllerFolder
{
    [Authorize]
    public class MessagesController : Controller
    {
        #region FieldsAndController
        private readonly ILogger<MessagesController> _logger;

        public readonly UserManager<AppUser> _userManager;

        public IMCHelper _helper { get; set; }

        public MessagesController(ILogger<MessagesController> logger, UserManager<AppUser> userManager, IMCHelper helper)
        {
            _logger = logger;
            _userManager = userManager;
            _helper = helper;
        }
        #endregion

        #region MainPage
        [Authorize]
        public async Task<IActionResult> MainPage(int currentPage=0)
        {
            if (currentPage <= 0)
            {
                currentPage = 1;
            }
            var username = HttpContext.User?.Identity?.Name ?? "";
            var sender = await _userManager.FindByNameAsync(username);
            if (sender != null)
            {
                var userMsgDTOList = await _helper.GetUsersWithMessages(sender.Id);
                return View("~/MessengerViews/Messages/MainPage.cshtml", userMsgDTOList.Take(20).Skip((currentPage - 1) * 20));
            }
            return View("~/MessengerViews/Messages/MainPage.cshtml");
        }
        #endregion

        #region Chat
        [Authorize]
        public async Task<IActionResult> Chat(int currentPage, string userName)
        {
            if (User!.Identity!.IsAuthenticated)
            {
                var username = HttpContext.User?.Identity?.Name ?? "";
                var currentUser = await _userManager.FindByNameAsync(username);

                var users = _userManager.Users;
                var specUserNeeded = users.Where(u => u.UserName == userName).FirstOrDefault();

                ViewBag.CurrentUserName = currentUser!.UserName;
                ViewBag.TargetedUserId = specUserNeeded!.Id;
                ViewBag.RelationshipBetweenThem = true;
                if (_helper.IsThereAreRelationshipBetweenThem(currentUser,specUserNeeded))
                {
                    ViewBag.RelationshipBetweenThem = true;
                }

                var messages = _helper.GetCurrentChatMessages(currentUser.Id, specUserNeeded!.Id);

                return View("~/MessengerViews/Messages/Chat.cshtml", messages);
            }
            return View("~/MessengerViews/Messages/MainPage.cshtml");
        }

        [HttpPost]
        public async Task Create(Messages message)
        {
            var username = HttpContext.User?.Identity?.Name ?? "";
            var user = await _userManager.FindByNameAsync(username);
            await _helper.CreateMessage(message, user!);
        }

        [HttpPost]
        public async Task<JsonResult> BanUser(string userId)
        {
            try
            {
                var receiverUsername = HttpContext.User?.Identity?.Name ?? "";
                var receiver = await _userManager.FindByNameAsync(receiverUsername);
                var sender = await _userManager.FindByIdAsync(userId);

                var result = _helper.BanAUser(receiver!,sender!);
                return Json(new { status = false, Message = result });
            }
            catch (Exception)
            {
                return Json(new { status = false });
                throw;
            }
        }
        #endregion

        #region Helpers

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View("~/MessengerViews/Messages/Privacy.cshtml");
        }
        #endregion
    }
}