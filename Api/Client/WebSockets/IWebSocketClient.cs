using Api.Messaging;
using System.Net.WebSockets;

namespace Api.Client.WebSockets;

public interface IWebSocketClient : IClient, IAsyncDisposable
{
    IAsyncEnumerable<IncomingPackage> ReadAllMessages(CancellationToken cancellationToken);
    Task Close(WebSocketCloseStatus closeStatus);
}
