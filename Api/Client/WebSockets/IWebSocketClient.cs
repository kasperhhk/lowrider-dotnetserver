using Api.Messaging.Models;
using System.Net.WebSockets;

namespace Api.Client.WebSockets;

public interface IWebSocketClient : ISendableClient<ReadOnlyMemory<byte>>, IAsyncDisposable
{
    IAsyncEnumerable<IncomingPackage> ReadAllMessages(CancellationToken cancellationToken);
    Task Close(WebSocketCloseStatus closeStatus);
}
