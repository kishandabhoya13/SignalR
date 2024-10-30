namespace SignalRDemo.Models
{
    public class CheckStatusDto
    {
        public int CheckId { get; set; }

        public string UserId { get; set; }

        public string Token { get; set; }

        public bool IsApproved { get; set; }

        public bool IsDeclined { get; set; }

    }
}
