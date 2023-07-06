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
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IDictionary<string, IFeatureHandler> _featureHandlers;
    private readonly IEnumerable<IClientSender> _clientSenders;
    private readonly ILogger<MessageBroker> _logger;

    public MessageBroker(IHostApplicationLifetime hostApplicationLifetime, IEnumerable<IFeatureHandler> featureHandlers, IEnumerable<IClientSender> clientSenders, ILogger<MessageBroker> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _featureHandlers = featureHandlers.ToDictionary(_ => _.Name);
        _clientSenders = clientSenders;
        _logger = logger;
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
        {
            try
            {
                await handler.Handle(sender, package, _hostApplicationLifetime.ApplicationStopping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling package: {Feature}, {Command}", package.Feature, package.Command);
            }
        }
    }
}