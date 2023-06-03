using System.Buffers;
using System.Net.WebSockets;

namespace K;

public interface IWebSocketSerializationStrategy
{
  WebSocketMessageType MessageType { get; }
  IInboundMessage Deserialize(ReadOnlySpan<byte> incoming);
  byte[] Serialize(IOutboundMessage message, out IMemoryOwner<byte> owner, out ReadOnlyMemory<byte> typeSegment);
}