namespace K;

public interface IOutboundMessageCentral
{
  void Deliver(IEnumerable<IClient> recipients, IOutboundMessage message);  
}

public interface IInboundMessageCentral {
  void Deliver(IUser sender, IInboundMessage message);
}