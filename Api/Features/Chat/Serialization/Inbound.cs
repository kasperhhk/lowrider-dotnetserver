using Api.Features.Serialization.Json;

namespace K;

public class InboundChatMessageSerialization : IJsonSerialization
{
  public string Type => InboundChatMessage.MessageType;

  public IInboundMessage Deserialize(IJsonValueFactory factory, ReadOnlySpan<char> payload)
    => factory.JsonDeserialize<InboundChatMessage>(payload);
}