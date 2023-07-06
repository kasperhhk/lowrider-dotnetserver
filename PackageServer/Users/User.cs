namespace Api.Users;

public interface IUser
{
    string Id { get; }
}

public record User(string Id) : IUser;