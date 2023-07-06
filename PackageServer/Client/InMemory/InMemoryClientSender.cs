using Api.Messaging;

namespace Api.Client.InMemory;

public class InMemoryClientSender : TypedClientSender<InMemoryClient, InMemoryOutgoingPackage>
{
    protected override InMemoryOutgoingPackage PrepareData<TPayload>(Messaging.Models.OutgoingPackage<TPayload> package)
    {
        return new InMemoryOutgoingPackage(package.Feature, package.Command, package.Payload);
    }
}
