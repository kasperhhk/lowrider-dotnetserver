using Api.Users;

namespace Api.Client.InMemory;

public interface IInMemoryClientFactory
{
    RegisteredInMemoryClient CreateAndRegister(IUser user);
}

public class InMemoryClientFactory : IInMemoryClientFactory
{
    private readonly ILogger<InMemoryClient> _clientLogger;
    private readonly IClientManager _clientManager;

    public InMemoryClientFactory(ILogger<InMemoryClient> clientLogger, IClientManager clientManager)
    {
        _clientLogger = clientLogger;
        _clientManager = clientManager;
    }

    public RegisteredInMemoryClient CreateAndRegister(IUser user)
    {
        var client = new InMemoryClient(user, _clientLogger);
        var registration = _clientManager.RegisterClient(client);

        return new RegisteredInMemoryClient(registration, client);
    }
}
