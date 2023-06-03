using System.Net.WebSockets;

namespace K;

public interface IWebSocketClientFactory
{
  IWebSocketClient CreateClient(WebSocket webSocket, IUser user);
}

public class WebSocketClientFactory : IWebSocketClientFactory
{
  private readonly ILogger<WebSocketClient> _logger;
  private readonly IWebSocketSerializationStrategy _webSocketSerializationStrategy;

  public WebSocketClientFactory(ILogger<WebSocketClient> logger, IWebSocketSerializationStrategy webSocketSerializationStrategy)
  {
    _logger = logger;
    _webSocketSerializationStrategy = webSocketSerializationStrategy;
  }

  public IWebSocketClient CreateClient(WebSocket webSocket, IUser user)
  {
    return new WebSocketClient(webSocket, user, _logger, _webSocketSerializationStrategy);
  }
}
