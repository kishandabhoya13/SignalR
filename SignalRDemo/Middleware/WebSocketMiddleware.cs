using System.Net.WebSockets;

namespace SignalRDemo.Middleware
{

    //USE MIDDLEWARE TO CONNECT SOCKETS

    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var handler = new ChatWebSocketHandler(webSocket, context);
                await handler.HandleAsync(context.RequestAborted);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
