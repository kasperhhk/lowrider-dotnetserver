using Api.Client;
using Api.Messaging.Models;

namespace Api.Messaging;

public interface IClientSender
{
    Task Send<TPayload>(IEnumerable<IClient> clients, OutgoingPackage<TPayload> package, CancellationToken cancellationToken) where TPayload : notnull;
}