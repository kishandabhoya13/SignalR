using System.Net.WebSockets;

namespace SignalRDemo.Models
{
    public class ChatMessage
    {
        public string UserId { get; set; }

        public string HodId { get; set; }
        
        public string Message { get; set; }

        public string Role { get; set; }

        public string ReceiverUsername { get; set; }

        public string Username { get; set; }
    }

    public class ChatSession
    {
        public string HodId { get; set; }
        public string StudentId { get; set; }
        public WebSocket HodSocket { get; set; }
        public WebSocket StudentSocket { get; set; }
    }
}
