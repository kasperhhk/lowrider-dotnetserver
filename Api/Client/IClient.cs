using Api.Users;

namespace Api.Client;

public interface IClient
{
    string Id { get; }
    IUser User { get; }
}