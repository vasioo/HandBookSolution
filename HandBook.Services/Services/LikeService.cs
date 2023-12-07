using HandBook.DataAccess;
using HandBook.Models;
using HandBook.Services.Interfaces;
using HandBook.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandBook.Services.Services
{
    internal class LikeService : BaseService<Likes>, ILikesService
    {
        private readonly ApplicationDbContext _dataContext;
        public LikeService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }
    }
}
