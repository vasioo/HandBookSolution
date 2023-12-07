using HandBook.DataAccess;
using HandBook.Services.Interfaces;
using HandBook.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandBook.Services.Services
{
    internal class FollowerService : BaseService<Followers>, IFollowerService
    {
        private readonly ApplicationDbContext _dataContext;
        public FollowerService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }
    }
}
