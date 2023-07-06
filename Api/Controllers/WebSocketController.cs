using System.Net.WebSockets;
using Api.Client.WebSockets;
using Api.Messaging;
using Api.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("ws")]
public class WebSocketController : ControllerBase
{
    private readonly IWebSocketClientFactory _clientFactory;
    private readonly ILogger<WebSocketController> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IMessageBroker _messageBroker;

    public WebSocketController(IWebSocketClientFactory clientFactory, ILogger<WebSocketController> logger, IHostApplicationLifetime hostApplicationLifetime, IMessageBroker messageBroker)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
        _messageBroker = messageBroker;
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
        await using var registeredClient = _clientFactory.CreateAndRegister(webSocket, user);
        var client = registeredClient.Client;

        _logger.LogInformation("User connection: {username}", username);

        try
        {
            await foreach (var message in client.ReadAllMessages(cancellationToken))
            {
                // fire and forget
                _ = _messageBroker.Send(client.User, message);
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