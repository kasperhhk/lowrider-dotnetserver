using Api.Users;

namespace Api.Messaging;

public interface IFeatureHandler
{
    string Name { get; }

    Task Handle(IUser sender, IncomingPackage package, CancellationToken cancellationToken);
}
