using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Api.Controllers;

[Route("ws")]
public class WebSocketController : ControllerBase
{
  private readonly IChatService _chatService;
  private readonly ILogger<WebSocketController> _logger;
  private readonly JsonSerializerOptions _jsonSerializerOptions;
  private readonly IHostApplicationLifetime _hostApplicationLifetime;

  public WebSocketController(IChatService chatService, ILogger<WebSocketController> logger, JsonSerializerOptions jsonSerializerOptions, IHostApplicationLifetime hostApplicationLifetime)
  {
    _chatService = chatService;
    _logger = logger;
    _jsonSerializerOptions = jsonSerializerOptions;
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
    try
    {
      using var chatClient = _chatService.RegisterClient(username);
      if (chatClient == null)
      {
        HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        return;
      }

      _logger.LogInformation("New user connected with username: {username}", username);

      // ToClient loop
      chatClient.OnMessage += async (message) =>
      {
        try
        {
          var json = JsonSerializer.Serialize(message, _jsonSerializerOptions);
          var bytes = System.Text.Encoding.UTF8.GetBytes(json);
          if (ws.State is WebSocketState.Open)
          {
            await ws.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error sending message to client.");
        }
      };

      // Welcome message
      chatClient.ToClient(new ServerChatMessage("server", "Welcome! :3"));

      // ToServer loop
      var readBuffer = new byte[1024];
      while (ws.State is WebSocketState.Open)
      {
        var received = await ws.ReceiveAsync(readBuffer, cancellationToken);
        if (received.CloseStatus is not null)
          break;

        var json = System.Text.Encoding.UTF8.GetString(readBuffer, 0, received.Count);
        var message = JsonSerializer.Deserialize<ClientChatMessage>(json, _jsonSerializerOptions);
        if (message == null)
          continue;

        chatClient.ToServer(message);
      }
    }
    catch (OperationCanceledException)
    {
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error handling client, closing client {username}.", username);
      await CloseWebSocket(ws, WebSocketCloseStatus.InternalServerError);
    }
    finally
    {
      _logger.LogInformation("Closing client {username}", username);
      await CloseWebSocket(ws, WebSocketCloseStatus.NormalClosure);
    }
  }

  private static async Task CloseWebSocket(WebSocket ws, WebSocketCloseStatus closeStatus)
  {
    var token = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
    if (ws.State is WebSocketState.CloseReceived)
    {
      await ws.CloseOutputAsync(closeStatus, null, token);
    }
    else if (ws.State is not (WebSocketState.CloseSent or WebSocketState.Aborted))
    {
      await ws.CloseAsync(closeStatus, null, token);
    }
  }
}

public static class ServiceCollectionExtension
{
  public static IServiceCollection AddChatFeature(this IServiceCollection serviceCollection)
    => serviceCollection
      .AddSingleton<IChatService, ChatService>();
}


public interface IChatService
{
  IChatClient? RegisterClient(string username);
  void Unregister(IChatClient client);
  void Send(ServerChatMessage message);
}

public interface IChatClient : IDisposable
{
  delegate void ChatEvent<T>(T evt);
  event ChatEvent<ServerChatMessage>? OnMessage;

  public string Username { get; }

  void ToServer(ClientChatMessage message);
  void ToClient(ServerChatMessage message);
}

public record ServerChatMessage(string sender, string message);
public record ClientChatMessage(string message);

public class ChatService : IChatService
{
  private readonly ConcurrentDictionary<IChatClient, string> _clients = new();

  public IChatClient? RegisterClient(string username)
  {
    var client = new ChatClient(username, this);
    if (_clients.TryAdd(client, username))
      return client;

    return null;
  }

  public void Send(ServerChatMessage message)
  {
    foreach (var client in _clients.Keys)
    {
      client.ToClient(message);
    }
  }

  public void Unregister(IChatClient client)
  {
    _clients.TryRemove(client, out _);
  }
}

public class ChatClient : IChatClient
{
  public event IChatClient.ChatEvent<ServerChatMessage>? OnMessage;

  private readonly string _username;
  private readonly IChatService _chatService;

  public string Username => _username;

  public ChatClient(string username, IChatService chatService)
  {
    _username = username;
    _chatService = chatService;
  }

  public void Dispose()
  {
    _chatService.Unregister(this);
  }

  public void ToClient(ServerChatMessage message)
  {
    OnMessage?.Invoke(message);
  }

  public void ToServer(ClientChatMessage message)
  {
    _chatService.Send(new ServerChatMessage(Username, message.message));
  }
}