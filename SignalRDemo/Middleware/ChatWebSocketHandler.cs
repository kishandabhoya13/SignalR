using Newtonsoft.Json;
using SignalRDemo.Models;
using StudentManagment.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace SignalRDemo.Middleware
{
    public class ChatWebSocketHandler
    {
        private static ConcurrentDictionary<WebSocket, string> _clients = new ConcurrentDictionary<WebSocket, string>();
        private readonly WebSocket _webSocket;
        private readonly HttpContext _httpContext;
        private static ConcurrentDictionary<string, bool> currentChating = new();
        public ChatWebSocketHandler(WebSocket webSocket, HttpContext httpContext)
        {
            _webSocket = webSocket;
            _httpContext = httpContext;
        }

        public async Task HandleAsync(CancellationToken token)
        {
            string userName = null;
            try
            {
                var buffer = new byte[1024 * 4];
                while (_webSocket.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var messageData = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var message = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatMessage>(messageData);
                        if (userName == null)
                        {
                            userName = message.Username;
                            _clients.TryAdd(_webSocket, userName);
                        }


                        if (!string.IsNullOrEmpty(message.ReceiverUsername))
                        {
                            // Find the target client based on TargetUserId
                            var targetClient = _clients.FirstOrDefault(c => c.Value == message.ReceiverUsername).Key;

                            if (targetClient != null)
                            {
                                var directMessage = JsonConvert.SerializeObject(message);
                                await targetClient.SendAsync(
                                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(directMessage)),
                                    WebSocketMessageType.Text,
                                    true,
                                    token
                                );
                            }
                        }
                    }
                }
            }
            finally
            {
                _clients.TryRemove(_webSocket, out _);
            }

        }
    }


    //public class ChatWebSocketHandler
    //{
    //    private static ConcurrentDictionary<string, WebSocket> _hodSockets = new();
    //    private static ConcurrentDictionary<string, WebSocket> _studentSockets = new();
    //    private static ConcurrentDictionary<string, ChatSession> _chatSessions = new();
    //    private readonly WebSocket _webSocket;
    //    private readonly HttpContext _httpContext;

    //    //public ChatWebSocketHandler(WebSocket webSocket, HttpContext httpContext)
    //    //{
    //    //    _webSocket = webSocket;
    //    //    _httpContext = httpContext;
    //    //}

    //    public async Task Handle(WebSocket webSocket, string userId, string role)
    //    {
    //        if (role == "HOD")
    //        {
    //            _hodSockets[userId] = _webSocket;
    //        }
    //        else
    //        {
    //            _studentSockets[userId] = _webSocket;
    //        }

    //        while (_webSocket.State == WebSocketState.Open)
    //        {
    //            var buffer = new byte[1024 * 4];
    //            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    //            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

    //            // Example: message format could be "studentId:message"
    //            var parts = message.Split(':', 2);
    //            if (parts.Length == 2)
    //            {
    //                string targetId = parts[0];
    //                string chatMessage = parts[1];

    //                if (role == "HOD" && _studentSockets.TryGetValue(targetId, out var studentSocket))
    //                {
    //                    await studentSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"HOD: {chatMessage}")), WebSocketMessageType.Text, true, CancellationToken.None);
    //                }
    //                else if (role == "Student" && _hodSockets.TryGetValue(targetId, out var hodSocket))
    //                {
    //                    await hodSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Student: {chatMessage}")), WebSocketMessageType.Text, true, CancellationToken.None);
    //                }
    //            }
    //        }

    //        // Clean up when done
    //        if (role == "HOD")
    //        {
    //            _hodSockets.TryRemove(userId, out _);
    //        }
    //        else
    //        {
    //            _studentSockets.TryRemove(userId, out _);
    //        }

    //        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
    //    }
    //}




    //public static class WebSocketHandler
    //{
    //    private static readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    //    public static async Task HandleWebSocket(WebSocket webSocket)
    //    {
    //        var id = Guid.NewGuid().ToString();
    //        _sockets.TryAdd(id, webSocket);

    //        await SendMessage(webSocket, $"Welcome to the call, your ID is {id}");

    //        while (webSocket.State == WebSocketState.Open)
    //        {
    //            var buffer = new byte[1024];
    //            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    //            if (result.MessageType == WebSocketMessageType.Close)
    //            {
    //                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    //                _sockets.TryRemove(id, out _);
    //            }
    //        }
    //    }

    //    public static async Task StartCall(string hostId)
    //    {
    //        var message = $"{hostId} has started a call.";
    //        foreach (var socket in _sockets)
    //        {
    //            await SendMessage(socket.Value, message);
    //        }
    //    }

    //    public static async Task JoinCall(string participantId, string hostId)
    //    {
    //        if (_sockets.TryGetValue(hostId, out var hostSocket))
    //        {
    //            await SendMessage(hostSocket, $"{participantId} wants to join.");
    //        }
    //    }

    //    private static async Task SendMessage(WebSocket socket, string message)
    //    {
    //        var bytes = Encoding.UTF8.GetBytes(message);
    //        await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    //    }
    //}
}
