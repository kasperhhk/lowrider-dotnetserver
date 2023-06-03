using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Api.Features.Serialization.Json;

namespace K;

public class JsonWebSocketSerializationStrategy : IWebSocketSerializationStrategy, IJsonValueFactory
{
  public const char TypeDelimiter = ':';
  public const int TypeDelimiterByteLength = 1;

  private readonly JsonSerializerOptions _jsonSerializerOptions;
  private readonly IDictionary<string, IJsonSerialization> _jsonSerializations;

  public JsonWebSocketSerializationStrategy(JsonSerializerOptions jsonSerializerOptions, IEnumerable<IJsonSerialization> jsonSerializations)
  {
    _jsonSerializerOptions = jsonSerializerOptions;
    _jsonSerializations = jsonSerializations.ToDictionary(_ => _.Type);
  }

  public WebSocketMessageType MessageType => WebSocketMessageType.Text;

  public IInboundMessage Deserialize(ReadOnlySpan<byte> incoming)
  {
    var size = Encoding.UTF8.GetCharCount(incoming);

    using var memory = MemoryPool<char>.Shared.Rent(size);
    var incomingStr = memory.Memory.Slice(0, size).Span;

    Encoding.UTF8.GetChars(incoming, incomingStr);

    var typeDelimiterIndex = incomingStr.IndexOf(TypeDelimiter);
    var type = incomingStr.Slice(0, typeDelimiterIndex).ToString();
    var payload = incomingStr.Slice(typeDelimiterIndex + 1);

    var serialization = _jsonSerializations[type];
    return serialization.Deserialize(this, payload);
  }

  public T JsonDeserialize<T>(ReadOnlySpan<char> payload)
  {
    var result = JsonSerializer.Deserialize<T>(payload, _jsonSerializerOptions);
    if (result == null)
      throw new JsonException("Deserialized null value");

    return result;
  }

  public byte[] Serialize(IOutboundMessage message, out IMemoryOwner<byte> owner, out ReadOnlyMemory<byte> typeSegment)
  {
    // OK i don't know how to manage the memory of the serialized payload at all, even if i use a memorystream, there's no way to know how big the buffer needs to be...
    var payloadBytes = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType(), _jsonSerializerOptions);

    // So we only manage the typesegment
    var typeLength = Encoding.UTF8.GetByteCount(message.Type);
    var totalLength = typeLength + TypeDelimiterByteLength;

    owner = MemoryPool<byte>.Shared.Rent(totalLength);
    var memorySlice = owner.Memory.Slice(0, totalLength);
    var typeSegmentSpan = memorySlice.Span;
    var typeSpan = typeSegmentSpan.Slice(0, typeLength);
    var delimiterSpan = typeSegmentSpan.Slice(typeLength, 1);

    Encoding.UTF8.GetBytes(message.Type, typeSpan);
    delimiterSpan[0] = (byte)TypeDelimiter;

    typeSegment = memorySlice;

    return payloadBytes;
  }
}