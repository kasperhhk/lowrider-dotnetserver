using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Api.Features.WebSockets;
using Api.Features.Users;
using Api.Features.Messaging;

namespace Api.Controllers;

[Route("ws")]
public class WebSocketController : ControllerBase
{
  private readonly IConnectionManager _connectionManager;
  private readonly WebSocketConnectionFactory _webSocketConnectionFactory;
  private readonly ILogger<WebSocketController> _logger;
  private readonly IHostApplicationLifetime _hostApplicationLifetime;

  public WebSocketController(IConnectionManager connectionManager, WebSocketConnectionFactory webSocketConnectionFactory, ILogger<WebSocketController> logger, IHostApplicationLifetime hostApplicationLifetime)
  {
    _connectionManager = connectionManager;
    _webSocketConnectionFactory = webSocketConnectionFactory;
    _logger = logger;
    _hostApplicationLifetime = hostApplicationLifetime;
  }

  [Route("{username}")]
  public async Task Get(string username)
  {
    if (HttpContext.WebSockets.IsWebSocketRequest)
    {
      using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
      await HandleClient(username, webSocket, _hostApplicationLifetime.ApplicationStopping);
    }
    else
    {
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
  }

  private async Task HandleClient(string username, WebSocket ws, CancellationToken cancellationToken)
  {
    var user = new User(username);
    await using var connection = _webSocketConnectionFactory.Create(user, ws);
    using var registration = ConnectionRegistrationToken.Register(_connectionManager, connection);
    _logger.LogInformation("User connection: {username}", username);

    try
    {
      await foreach (var message in connection.ReadAllMessages(cancellationToken))
      {
        HandleMessage(user, message, cancellationToken);
      }
    }
    catch (OperationCanceledException)
    {
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error handling client, closing client {username}.", username);
      await connection.Close(WebSocketCloseStatus.InternalServerError);
    }
  }

  private void HandleMessage(User sender, IMessage message, CancellationToken cancellationToken)
  {
    var chat = (IncomingChatMessage)message;
    foreach (var conn in _connectionManager.GetConnections())
    {
      Task.Run(async () =>
      {
        await conn.Write(new OutgoingChatMessage(sender.Username, chat.message), cancellationToken);
      });
    }
  }
}