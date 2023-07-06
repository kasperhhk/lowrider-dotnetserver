using Api.Client;
using Api.Serialization;
using Api.Users;

namespace Api.Messaging;

public interface IMessageBroker
{
    Task Send<TPayload>(Recipients.Recipients recipients, OutgoingPackage<TPayload> package) where TPayload : notnull;
    Task Send(IUser sender, IncomingPackage package);
}

public class MessageBroker : IMessageBroker
{
    private readonly IClientManager _clientManager;
    private readonly IPackageSerializer _packageSerializer;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IDictionary<string, IFeatureHandler> _featureHandlers;

    public MessageBroker(IClientManager clientManager, IPackageSerializer packageSerializer, IHostApplicationLifetime hostApplicationLifetime, IEnumerable<IFeatureHandler> featureHandlers)
    {
        _clientManager = clientManager;
        _packageSerializer = packageSerializer;
        _hostApplicationLifetime = hostApplicationLifetime;
        _featureHandlers = featureHandlers.ToDictionary(_ => _.Name);
    }

    public Task Send<TPayload>(Recipients.Recipients recipients, OutgoingPackage<TPayload> package) where TPayload : notnull
    {
        var serializedPackage = _packageSerializer.Serialize(package);

        foreach (var client in recipients.Clients)
        {
            // Fire and forget
            client.Send(serializedPackage, _hostApplicationLifetime.ApplicationStopping);
        }

        return Task.CompletedTask;
    }

    public async Task Send(IUser sender, IncomingPackage package)
    {
        if (_featureHandlers.TryGetValue(package.Feature, out var handler))
            await handler.Handle(sender, package, _hostApplicationLifetime.ApplicationStopping);
    }
}