namespace K;

public interface IMessageHandler
{
  bool CanHandle(IInboundMessage message);
  void Handle(IUser sender, IInboundMessage message);
}