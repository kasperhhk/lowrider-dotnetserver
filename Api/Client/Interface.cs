namespace Api.Client;

public interface ISendableClient<TClientModel> : IClient
{
    Task Send(TClientModel data, CancellationToken cancellationToken);
}
