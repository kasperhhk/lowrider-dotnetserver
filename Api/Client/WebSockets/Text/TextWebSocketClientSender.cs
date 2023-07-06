using Api.Messaging;
using Api.Messaging.Models;
using Api.Serialization.Json;

namespace Api.Client.WebSockets.Text;

public class TextWebSocketClientSender : TypedClientSender<TextWebSocketClient, ReadOnlyMemory<byte>>
{
    private readonly IJsonPackageSerializer _serializer;

    public TextWebSocketClientSender(IJsonPackageSerializer serializer)
    {
        _serializer = serializer;
    }

    protected override ReadOnlyMemory<byte> PrepareData<TPayload>(OutgoingPackage<TPayload> package)
    {
        return _serializer.Serialize(package);
    }
}
