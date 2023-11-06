using System.ComponentModel.DataAnnotations;

namespace Messenger.Models
{
    public class Messages
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime TimeSent { get; set; }

        public string UserId { get; set; }
        
        public string ReceiverId { get; set; }

        public virtual AppUser Sender { get; set; }

        public Messages()
        {
            TimeSent = DateTime.Now;
        }
    }
}
