using Messenger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandBook.Models.BaseModels.Interfaces
{
    public interface IUser
    {
        string Gender { get; set; }
        ICollection<Messages> Messages { get; set; }
    }
}
