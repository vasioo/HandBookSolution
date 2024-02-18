namespace Messenger.Models
{
    public class UserMassageDTO
    {
        public string UserData { get; set; } = "";

        public string Message { get; set; } = "";

        public DateTime DateOfSending { get; set; } = DateTime.Now;
    }
}
