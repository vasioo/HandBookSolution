using HandBook.DataAccess;
using HandBook.Web.Extensions;
using Messenger.Hubs;
using Messenger.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HandBook.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                });
            });

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();


            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddDefaultIdentity<AppUser>(
              options =>
              {
                  options.SignIn.RequireConfirmedAccount = false;
                  options.Password.RequireDigit = false;
                  options.Password.RequireLowercase = false;
                  options.Password.RequireNonAlphanumeric = false;
                  options.Password.RequireUppercase = false;
              })
                 .AddRoles<IdentityRole>()
                 .AddEntityFrameworkStores<ApplicationDbContext>()
                 .AddDefaultTokenProviders()
                 .AddDefaultUI()
                 .AddSignInManager<SignInManager<AppUser>>()
                 .AddUserManager<UserManager<AppUser>>()
                 .AddRoleManager<RoleManager<IdentityRole>>()
                 .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddSignalR();


            var facebookAppId = builder.Configuration.GetSection("Facebook:AppId").Get<string>() ?? "";
            var facebookAppSecret = builder.Configuration.GetSection("Facebook:AppSecret").Get<string>() ?? "";
            var googleClientId = builder.Configuration.GetSection("Google:ClientId").Get<string>() ?? "";
            var googleClientSecret = builder.Configuration.GetSection("Google:ClientSecret").Get<string>() ?? "";


            builder.Services.AddAuthentication()
                        .AddFacebook(options =>
                        {
                            options.AppId = facebookAppId;
                            options.AppSecret = facebookAppSecret;
                        })
                        .AddGoogle(options =>
                        {
                            options.ClientId = googleClientId;
                            options.ClientSecret = googleClientSecret;
                        });
            builder.Services.AddControllersWithViews();

            builder.Services.AddApplication();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Messages}/{action=Index}/{id?}");
            app.MapRazorPages();
            app.MapHub<ChatHub>("/chatHub");
            app.Run();
        }
    }
}