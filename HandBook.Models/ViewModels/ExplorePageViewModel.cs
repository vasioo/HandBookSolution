using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandBook.Models.ViewModels
{
    public class ExplorePageViewModel
    {
        public IQueryable<Post> Posts { get; set; }
    }
}
