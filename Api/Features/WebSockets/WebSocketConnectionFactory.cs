using System.Net.WebSockets;
using Api.Features.Users;

namespace Api.Features.WebSockets;

public class WebSocketConnectionFactory
{
  private readonly IMessageSerializer _messageSerializer;
  private readonly ILogger<WebSocketConnection> _logger;

  public WebSocketConnectionFactory(IMessageSerializer messageSerializer, ILogger<WebSocketConnection> logger)
  {
    _messageSerializer = messageSerializer;
    _logger = logger;
  }

  public WebSocketConnection Create(User user, WebSocket webSocket)
  {
    return new WebSocketConnection(user, webSocket, _logger, _messageSerializer);
  }
}