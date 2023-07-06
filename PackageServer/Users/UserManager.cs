using System.Collections.Concurrent;

namespace Api.Users;

public interface IUserManager
{
    IQueryable<IUser> Users { get; }
    bool TryAddUser(IUser user);
}

public class UserManager : IUserManager
{
    private readonly ConcurrentDictionary<string, IUser> _users = new();
    public IQueryable<IUser> Users => _users.Values.AsQueryable();

    public bool TryAddUser(IUser user)
    {
        return _users.TryAdd(user.Id, user);
    }

    public IUser GetUser(string id)
    {
        if (_users.TryGetValue(id, out IUser? user))
            return user;

        var newUser = new User(id);
        if (_users.TryAdd(id, newUser))
            return newUser;

        return _users[id];
    }
}