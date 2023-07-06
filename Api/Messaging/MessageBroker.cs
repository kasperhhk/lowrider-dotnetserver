using Api.Client;
using Api.Messaging.Models;
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
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IDictionary<string, IFeatureHandler> _featureHandlers;
    private readonly IEnumerable<IClientSender> _clientSenders;

    public MessageBroker(IClientManager clientManager, IHostApplicationLifetime hostApplicationLifetime, IEnumerable<IFeatureHandler> featureHandlers, IEnumerable<IClientSender> clientSenders)
    {
        _clientManager = clientManager;
        _hostApplicationLifetime = hostApplicationLifetime;
        _featureHandlers = featureHandlers.ToDictionary(_ => _.Name);
        _clientSenders = clientSenders;
    }

    public Task Send<TPayload>(Recipients.Recipients recipients, OutgoingPackage<TPayload> package) where TPayload : notnull
    {
        foreach (var sender in _clientSenders)
        {
            // Fire and forget
            sender.Send(recipients.Clients, package, _hostApplicationLifetime.ApplicationStopping);
        }

        return Task.CompletedTask;
    }

    public async Task Send(IUser sender, IncomingPackage package)
    {
        if (_featureHandlers.TryGetValue(package.Feature, out var handler))
            await handler.Handle(sender, package, _hostApplicationLifetime.ApplicationStopping);
    }
}