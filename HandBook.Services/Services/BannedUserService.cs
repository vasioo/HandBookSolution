using HandBook.DataAccess;
using HandBook.Models.UserModel;
using HandBook.Services.Interfaces;

namespace HandBook.Services.Services
{
    public class BannedUserService:BaseService<BannedUser>,IBannedUserService
    {
        private readonly ApplicationDbContext _dataContext;
        public BannedUserService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }
    }
}
