using System.ComponentModel.DataAnnotations;

namespace HandBook.Models.UserModel
{
    public class IdentityModel
    {
        [Required]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

        [Required]
        [EmailAddress]
        public string RegisterEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string RegisterPassword { get; set; }

        [Required]
        public string RegisterUsername { get; set; }
    }
}
