namespace Api.Client.InMemory;

public class RegisteredInMemoryClient : IDisposable
{
    private readonly IDisposable _registrationToken;
    public InMemoryClient Client { get; }

    public RegisteredInMemoryClient(IDisposable registrationToken, InMemoryClient client)
    {
        _registrationToken = registrationToken;
        Client = client;
    }

    public void Dispose()
    {
        _registrationToken.Dispose();
    }
}
