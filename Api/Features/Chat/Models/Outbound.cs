namespace K;

public class OutboundChatMessage : IOutboundMessage
{
  public const string Type = "CHATMSG";

  string IOutboundMessage.Type => Type;

  public string Message { get; set; } = default!;
  public string Sender { get; set; } = default!;
}