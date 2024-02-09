using System.ComponentModel.DataAnnotations;

namespace HandBook.Models.BaseModels.Interfaces
{
    public interface IEntity
    {
        [Key]
        Guid Id { get; set; }
    }
}
