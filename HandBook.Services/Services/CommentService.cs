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
    public class CommentService:BaseService<Comment>,ICommentService
    {
        private readonly ApplicationDbContext _dataContext;
        public CommentService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }

    }
}
