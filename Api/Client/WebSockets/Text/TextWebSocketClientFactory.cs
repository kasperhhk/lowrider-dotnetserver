using Api.Serialization.Json;
using Api.Users;
using System.Net.WebSockets;

namespace Api.Client.WebSockets.Text;

public class TextWebSocketClientFactory : IWebSocketClientFactory
{
    private readonly ILogger<TextWebSocketClient> _clientLogger;
    private readonly IJsonPackageSerializer _packageSerializer;
    private readonly IClientManager _clientManager;

    public TextWebSocketClientFactory(ILogger<TextWebSocketClient> clientLogger, IJsonPackageSerializer packageSerializer, IClientManager clientManager)
    {
        _clientLogger = clientLogger;
        _packageSerializer = packageSerializer;
        _clientManager = clientManager;
    }

    public RegisteredWebSocketClient CreateAndRegister(WebSocket webSocket, IUser user)
    {
        var client = new TextWebSocketClient(webSocket, user, _clientLogger, _packageSerializer);
        var registration = _clientManager.RegisterClient(client);

        return new RegisteredWebSocketClient(registration, client);
    }
}
