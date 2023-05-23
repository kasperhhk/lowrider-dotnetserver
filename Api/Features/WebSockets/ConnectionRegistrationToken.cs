using Api.Features.Messaging;
using Api.Features.Users;

namespace Api.Features.WebSockets;

public class ConnectionRegistrationToken : IRegistrationToken
{
  public Guid Id { get; } = Guid.NewGuid();

  public User User { get; }

  public readonly IConnectionManager _connectionManager;

  private ConnectionRegistrationToken(User user, IConnectionManager connectionManager)
  {
    User = user;
    _connectionManager = connectionManager;    
  }

  public void Dispose()
  {
    _connectionManager.Unregister(this);
  }

  public static ConnectionRegistrationToken Register(IConnectionManager connectionManager, WebSocketConnection connection)
  {
    var token = new ConnectionRegistrationToken(connection.User, connectionManager);
    connectionManager.Register(token, connection);

    return token;
  }
}