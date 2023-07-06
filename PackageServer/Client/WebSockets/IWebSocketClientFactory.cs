using Api.Users;
using System.Net.WebSockets;

namespace Api.Client.WebSockets;

public interface IWebSocketClientFactory
{
    RegisteredWebSocketClient CreateAndRegister(WebSocket webSocket, IUser user);
}
