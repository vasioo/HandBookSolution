using HandBook.DataAccess;
using Messenger.Models;
using Microsoft.AspNet.Identity;

namespace HandBook.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddApplication(this IServiceCollection service)
        {
            service.AddScoped<ApplicationDbContext, ApplicationDbContext>();

            service.AddScoped<UserManager<AppUser>>();
        }
    }
}
