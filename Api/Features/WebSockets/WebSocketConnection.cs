using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Api.Features.Messaging;
using Api.Features.Users;

namespace Api.Features.WebSockets;

public class WebSocketConnection : IAsyncDisposable
{
  public User User => _user;

  private readonly User _user;
  private readonly WebSocket _webSocket;
  private readonly ILogger<WebSocketConnection> _logger;
  private readonly IMessageSerializer _messageSerializer;

  private readonly byte[] _buffer = new byte[1024];

  public WebSocketConnection(User user, WebSocket webSocket, ILogger<WebSocketConnection> logger, IMessageSerializer messageSerializer)
  {
    _user = user;
    _webSocket = webSocket;
    _logger = logger;
    _messageSerializer = messageSerializer;
  }

  public async Task Write(IMessage message, CancellationToken cancellationToken)
  {
    try
    {
      if (_webSocket.State is WebSocketState.Open)
      {
        var bytes = _messageSerializer.Serialize(message);
        await _webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellationToken);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error sending message to client {username}.", _user.Username);
    }
  }

  public async IAsyncEnumerable<IMessage> ReadAllMessages([EnumeratorCancellation] CancellationToken cancellationToken)
  {
    while (_webSocket.State is WebSocketState.Open)
    {
      var received = await _webSocket.ReceiveAsync(_buffer, cancellationToken);
      if (received.CloseStatus is not null)
        break;

      var message = _messageSerializer.Deserialize(_buffer, 0, received.Count);
      if (message == null)
        continue;

      yield return message;
    }
  }

  public async Task Close(WebSocketCloseStatus closeStatus)
  {
    var token = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
    if (_webSocket.State is WebSocketState.CloseReceived)
    {
      _logger.LogInformation("Client closed connection {username}", _user.Username);
      await _webSocket.CloseOutputAsync(closeStatus, null, token);
    }
    else if (_webSocket.State is not (WebSocketState.CloseSent or WebSocketState.Aborted or WebSocketState.Closed))
    {
      _logger.LogInformation("Closing connection {username}", _user.Username);
      await _webSocket.CloseAsync(closeStatus, null, token);
    }
  }

  public async ValueTask DisposeAsync()
  {
    await Close(WebSocketCloseStatus.NormalClosure);
  }
}

public interface IMessageSerializer {
  byte[] Serialize(IMessage message);
  IMessage? Deserialize(byte[] data, int start, int length);
}

public class JsonChatMessageSerializer : IMessageSerializer {
  private readonly JsonSerializerOptions _jsonSerializerOptions;

  public JsonChatMessageSerializer(JsonSerializerOptions jsonSerializerOptions)
  {
    _jsonSerializerOptions = jsonSerializerOptions;
  }

  public IMessage? Deserialize(byte[] data, int start, int length)
  {
    var json = System.Text.Encoding.UTF8.GetString(data, start, length);
    var message = JsonSerializer.Deserialize<IncomingChatMessage>(json, _jsonSerializerOptions);
    return message;
  }

  public byte[] Serialize(IMessage message)
  {
    var json = JsonSerializer.Serialize((OutgoingChatMessage)message, _jsonSerializerOptions);
    var bytes = System.Text.Encoding.UTF8.GetBytes(json);
    return bytes;
  }
}