using System.Net.WebSockets;
using K;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("ws")]
public class WebSocketController : ControllerBase
{
  private readonly IWebSocketClientFactory _clientFactory;
  private readonly IClientManager _clientManager;
  private readonly ILogger<WebSocketController> _logger;
  private readonly IHostApplicationLifetime _hostApplicationLifetime;
  private readonly IMessageCentral _messageCentral;

  public WebSocketController(IWebSocketClientFactory clientFactory, IClientManager clientManager, ILogger<WebSocketController> logger, IHostApplicationLifetime hostApplicationLifetime, IMessageCentral messageCentral)
  {
    _clientFactory = clientFactory;
    _clientManager = clientManager;
    _logger = logger;
    _hostApplicationLifetime = hostApplicationLifetime;
    _messageCentral = messageCentral;
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

  public async Task HandleClient(string username, WebSocket webSocket, CancellationToken cancellationToken)
  {
    var user = new User(username);
    await using var client = _clientFactory.CreateClient(webSocket, user);
    using var registration = _clientManager.RegisterClient(client);

    _logger.LogInformation("User connection: {username}", username);

    try
    {
      await foreach (var message in client.ReadAllMessages(cancellationToken))
      {
        _messageCentral.Deliver(client.User, message);
      }
    }
    catch (OperationCanceledException)
    {
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error handling client, closing client {username}.", username);
      await client.Close(WebSocketCloseStatus.InternalServerError);
    }
    finally
    {
      await client.Close(WebSocketCloseStatus.NormalClosure);
    }
  }
}