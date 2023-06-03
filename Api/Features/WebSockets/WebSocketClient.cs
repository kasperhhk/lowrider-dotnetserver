using System.Buffers;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;

namespace K;

public interface IWebSocketClient : IClient, IAsyncDisposable
{
  Task WriteMessage(ReadOnlyMemory<byte> message, WebSocketMessageType messageType, bool isLastPart, CancellationToken cancellationToken);
  Task Close(WebSocketCloseStatus closeStatus);
}

public class WebSocketClient : IWebSocketClient
{
  public const int FrameSegmentSize = 1024;
  public const int MaxFrameSize = FrameSegmentSize * 4;

  private readonly WebSocket _webSocket;
  private readonly IWebSocketSerializationStrategy _serializationStrategy;
  private readonly ILogger<WebSocketClient> _logger;

  public string Id { get; }
  public IUser User { get; }

  public WebSocketClient(WebSocket webSocket, IUser user, ILogger<WebSocketClient> logger, IWebSocketSerializationStrategy serializationStrategy)
  {
    Id = Guid.NewGuid().ToString("N");
    User = user;
    _webSocket = webSocket;
    _logger = logger;
    _serializationStrategy = serializationStrategy;
  }

  public async Task WriteMessage(ReadOnlyMemory<byte> message, WebSocketMessageType messageType, bool isLastPart, CancellationToken cancellationToken)
  {
    try
    {
      if (_webSocket is not { State: WebSocketState.Open })
        return;

      for (var i = 0; i < message.Length; i += FrameSegmentSize)
      {
        var length = Math.Min(message.Length - i, FrameSegmentSize);
        var slice = message.Slice(i, length);
        var isLastSubSegment = i + length == message.Length;

        await _webSocket.SendAsync(slice, messageType, isLastSubSegment && isLastPart, cancellationToken);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error sending message to client {userId}.", User.Id);
    }
  }

  public async IAsyncEnumerable<IInboundMessage> ReadAllMessages([EnumeratorCancellation] CancellationToken cancellationToken)
  {
    using WebSocketMessageFrame messageFrame = new WebSocketMessageFrame();

    while (_webSocket.State is WebSocketState.Open)
    {
      var bufferOwner = MemoryPool<byte>.Shared.Rent(FrameSegmentSize);
      var buffer = bufferOwner.Memory;

      var received = await _webSocket.ReceiveAsync(buffer, cancellationToken);
      if (received.MessageType == WebSocketMessageType.Close)
      {
        bufferOwner.Dispose();
        break;
      }

      var receivedBuffer = buffer.Slice(0, received.Count);
      messageFrame.Add(bufferOwner, receivedBuffer);

      if (!received.EndOfMessage)
        continue;

      messageFrame.GetMessageSequence(out var messageSequence);

      IInboundMessage message;
      if (messageSequence.IsSingleSegment)
      {
        message = _serializationStrategy.Deserialize(messageSequence.FirstSpan);
      }
      else
      {
        var messageFrameSize = messageFrame.Size;
        if (messageFrameSize is > MaxFrameSize or > int.MaxValue)
        {
          throw new WebSocketException($"Message too long {messageFrame.Size} > {MaxFrameSize}");
        }

        var rentSize = (int)messageFrameSize;
        using var contiguousMemoryForMyOwnSanity = MemoryPool<byte>.Shared.Rent(rentSize);

        messageSequence.CopyTo(contiguousMemoryForMyOwnSanity.Memory.Span);

        message = _serializationStrategy.Deserialize(contiguousMemoryForMyOwnSanity.Memory.Span);
      }

      messageFrame.Reset();

      yield return message;
    }
  }

  public async Task Close(WebSocketCloseStatus closeStatus)
  {
    var token = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
    if (_webSocket.State is WebSocketState.CloseReceived)
    {
      _logger.LogInformation("Client closed connection {userId}", User.Id);
      await _webSocket.CloseOutputAsync(closeStatus, null, token);
    }
    else if (_webSocket.State is not (WebSocketState.CloseSent or WebSocketState.Aborted or WebSocketState.Closed))
    {
      _logger.LogInformation("Closing connection {userId}", User.Id);
      await _webSocket.CloseAsync(closeStatus, null, token);
    }
  }

  public async ValueTask DisposeAsync()
  {
    await Close(WebSocketCloseStatus.NormalClosure);
  }
}