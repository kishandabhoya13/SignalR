using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace SignalRDemoAPI.Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connectedClients = new();
        public async Task Connect(string aspNetUserId)
        {
            // Add the userId with the connectionId
            _connectedClients[aspNetUserId] = Context.ConnectionId;
            LogConnectedClients();

            // Optionally, send a welcome message to the user
            await Clients.Caller.SendAsync("Welcome", $"Connected as {aspNetUserId}");
        }

        private void LogConnectedClients()
        {
            Console.WriteLine("Connected Clients:");
            foreach (var kvp in _connectedClients)
            {
                Console.WriteLine($"- ConnectionId: {kvp.Key}, UserId: {kvp.Value}");
            }
        }

        public static string GetConnectionId(string userId)
        {
            string connectionId = "";
            if (_connectedClients.ContainsKey(userId))
            {
                connectionId = _connectedClients[userId];
            }
            return connectionId;
        }

        public async Task SendNotification(string username, string message)
        {
            string connectionId = GetConnectionId(username);
            if (connectionId != "")
            {
                await Clients.Client(connectionId).SendAsync("Notifications", message);
            }
        }
    }
}
