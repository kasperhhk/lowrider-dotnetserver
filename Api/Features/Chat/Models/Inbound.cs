namespace K;

public class InboundChatMessage : IInboundMessage
{
  public const string MessageType = "CHATMSG";

  public string Message { get; set; } = default!;
}