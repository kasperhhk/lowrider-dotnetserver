using Api.Client.WebSockets.Memory;
using Api.Messaging;
using Api.Serialization;
using Api.Users;
using System.Buffers;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;

namespace Api.Client.WebSockets;

public class TextWebSocketClient : IWebSocketClient
{
    public const int FrameSegmentSize = 1024;
    public const int MaxFrameSize = FrameSegmentSize * 4;

    private readonly WebSocket _webSocket;
    private readonly ILogger<TextWebSocketClient> _logger;
    private readonly IPackageSerializer _packageSerializer;

    public IUser User { get; }
    public string Id { get; }

    public TextWebSocketClient(WebSocket webSocket, IUser user, ILogger<TextWebSocketClient> logger, IPackageSerializer packageSerializer)
    {
        _webSocket = webSocket;
        User = user;
        Id = Guid.NewGuid().ToString("D");
        _logger = logger;
        _packageSerializer = packageSerializer;
    }

    public async Task Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        try
        {
            if (_webSocket is not { State: WebSocketState.Open })
                return;

            for (var i = 0; i < data.Length; i += FrameSegmentSize)
            {
                var length = Math.Min(data.Length - i, FrameSegmentSize);
                var slice = data.Slice(i, length);
                var isLastSegment = i + length == data.Length;

                await _webSocket.SendAsync(slice, WebSocketMessageType.Text, isLastSegment, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to client {userId}.", User.Id);
        }
    }

    public async IAsyncEnumerable<IncomingPackage> ReadAllMessages([EnumeratorCancellation] CancellationToken cancellationToken)
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

            IncomingPackage message;
            if (messageSequence.IsSingleSegment)
            {
                message = _packageSerializer.Deserialize(messageSequence.FirstSpan);
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

                message = _packageSerializer.Deserialize(contiguousMemoryForMyOwnSanity.Memory.Span);
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
