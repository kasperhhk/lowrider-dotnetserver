namespace K;

public interface IMessageCentral
{
  void Deliver(IEnumerable<IClient> recipients, IOutboundMessage message);
  void Deliver(IUser sender, IInboundMessage message);
}
