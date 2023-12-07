using System.ComponentModel.DataAnnotations;

namespace HandBook.Models.BaseModels.Interfaces
{
    public interface IEntity
    {
        [Key]
        int Id { get; set; }
    }
}
