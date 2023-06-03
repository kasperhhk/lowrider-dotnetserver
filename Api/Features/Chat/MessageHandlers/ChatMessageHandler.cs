namespace K;

public class ChatMessageHandler : IMessageHandler
{
  private readonly IMessageCentral _messageCentral;
  private readonly IClientManager _clientManager;

  public ChatMessageHandler(IMessageCentral messageCentral, IClientManager clientManager)
  {
    _messageCentral = messageCentral;
    _clientManager = clientManager;
  }

  public bool CanHandle(IInboundMessage message)
  {
    return message is InboundChatMessage;
  }

  public void Handle(IUser sender, IInboundMessage message)
  {
    var chatMessage = (InboundChatMessage)message;
    var outboundChatMessage = new OutboundChatMessage {
      Message = chatMessage.Message,
      Sender = sender.Id
    };

    var recipients = _clientManager.Clients;
    _messageCentral.Deliver(recipients, outboundChatMessage);
  }
}