namespace K;

public interface IClient
{
  string Id { get; }
  IUser User { get; }
  
  IAsyncEnumerable<IInboundMessage> ReadAllMessages(CancellationToken cancellationToken);
}
