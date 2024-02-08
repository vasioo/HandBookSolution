using HandBook.Web.Models;

namespace HandBook.Models.ViewModels
{
    public class ExplorePageViewModel
    {
        public IQueryable<CardDTO> Posts { get; set; }
    }
}
