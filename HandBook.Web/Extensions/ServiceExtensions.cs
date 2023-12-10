using HandBook.DataAccess;
using HandBook.Models;
using HandBook.Services.Interfaces;
using HandBook.Services.Services;
using HandBook.Web.Controllers.HomeControllerFolder;
using HandBook.Web.Controllers.MessagesControllerFolder;
using HandBook.Web.Models;
using Messenger.Models;
namespace HandBook.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplication(this IServiceCollection service)
        {
            service.AddScoped<ApplicationDbContext, ApplicationDbContext>();

            service.AddScoped<IBaseService<Comment>, BaseService<Comment>>();
            service.AddScoped<ICommentService, CommentService>();

            service.AddScoped<IBaseService<Followers>, BaseService<Followers>>();
            service.AddScoped<IFollowerService, FollowerService>();

            service.AddScoped<IBaseService<Likes>, BaseService<Likes>>();
            service.AddScoped<ILikesService, LikeService>();

            service.AddScoped<IBaseService<Messages>, BaseService<Messages>>();
            service.AddScoped<IMessageService, MessageService>();

            service.AddScoped<IBaseService<Notification>, BaseService<Notification>>();
            service.AddScoped<INotificationService, NotificationService>();

            service.AddScoped<IBaseService<Post>, BaseService<Post>>();
            service.AddScoped<IPostService,PostService>();

            service.AddScoped<IHCHelper, HCHelper>();
            service.AddScoped<IMCHelper, MCHelper>();

        }
    }
}
