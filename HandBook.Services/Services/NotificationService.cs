﻿using HandBook.DataAccess;
using HandBook.Models;
using HandBook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandBook.Services.Services
{
    internal class NotificationService : BaseService<Notification>, INotificationService
    {
        private readonly ApplicationDbContext _dataContext;
        public NotificationService(ApplicationDbContext context) : base(context)
        {
            _dataContext = context;
        }
    }
}
