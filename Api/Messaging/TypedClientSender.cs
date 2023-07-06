using Api.Client;
using Api.Messaging.Models;

namespace Api.Messaging;

public abstract class TypedClientSender<TClient, TClientModel> : IClientSender where TClient : ISendableClient<TClientModel>
{
    protected abstract TClientModel PrepareData<TPayload>(OutgoingPackage<TPayload> package) where TPayload : notnull;

    public Task Send<TPayload>(IEnumerable<IClient> clients, OutgoingPackage<TPayload> package, CancellationToken cancellationToken) where TPayload : notnull
    {
        var data = PrepareData(package);
        foreach (var client in clients.Where(c => c is TClient).Cast<TClient>())
        {
            // Fire and forget
            client.Send(data, cancellationToken);
        }

        return Task.CompletedTask;
    }
}
