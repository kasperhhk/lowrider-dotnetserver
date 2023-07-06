using Api.Users;
using System.Collections.Concurrent;

namespace Api.Client;

public interface IClientManager
{
    IQueryable<IClient> Clients { get; }
    IDisposable RegisterClient(IClient client);
    void UnregisterClient(IClient client);
}

public class ClientRegistrationDisposer : IDisposable
{
    private readonly IClient _client;
    private readonly IClientManager _clientManager;

    public ClientRegistrationDisposer(IClient client, IClientManager clientManager)
    {
        _client = client;
        _clientManager = clientManager;
    }

    public void Dispose()
    {
        _clientManager.UnregisterClient(_client);
    }
}

public class ClientManager : IClientManager
{
    private readonly IUserManager _userManager;
    private readonly ConcurrentDictionary<string, IClient> _clients = new();

    public IQueryable<IClient> Clients => _clients.Values.AsQueryable();

    public ClientManager(IUserManager userManager)
    {
        _userManager = userManager;
    }

    public IDisposable RegisterClient(IClient client)
    {
        _clients.AddOrUpdate(client.Id, id => client, (id, existing) => client);
        _userManager.TryAddUser(client.User);

        return new ClientRegistrationDisposer(client, this);
    }

    public void UnregisterClient(IClient client)
    {
        _clients.TryRemove(client.Id, out _);
    }
}