using Api.Features.Messaging;
using System.Collections.Concurrent;

namespace Api.Features.WebSockets;

public interface IConnectionManager
{
  void Register(IRegistrationToken registrationToken, WebSocketConnection connection);
  void Unregister(IRegistrationToken registration);
  IEnumerable<WebSocketConnection> GetConnections();
}

public class ConnectionManager : IConnectionManager
{
  private readonly ConcurrentDictionary<IRegistrationToken, WebSocketConnection> _connections = new();

  public IEnumerable<WebSocketConnection> GetConnections()
  {
    return _connections.Values;
  }

  public void Register(IRegistrationToken registrationToken, WebSocketConnection connection)
  {
    _connections.AddOrUpdate(registrationToken, add => connection, (update, existing) => connection);
  }

  public void Unregister(IRegistrationToken registrationToken)
  {
    _connections.TryRemove(registrationToken, out _);
  }
}