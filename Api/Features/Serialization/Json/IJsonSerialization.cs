using K;

namespace Api.Features.Serialization.Json;

public interface IJsonSerialization
{
  string Type { get; }
  IInboundMessage Deserialize(IJsonValueFactory factory, ReadOnlySpan<char> payload);
}