using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Messenger.Models
{
    public class Messages
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Text { get; set; } = "";

        public DateTime TimeSent { get; set; } = DateTime.UtcNow;

        public string SenderMessageId { get; set; } = "";

        public string MessageReceiverId { get; set; } = "";

        public bool IsRead { get; set; } = false;

    }
}
