﻿using HandBook.Web.Models;

namespace HandBook.Services.Interfaces
{
    public interface ICommentService : IBaseService<Comment>
    {
        Comment GetCommentBasedOnRandomGuid(string randomGuid);
    }
}
