namespace Api.Client.WebSockets;

public class RegisteredWebSocketClient : IAsyncDisposable
{
    private readonly IDisposable _registrationToken;
    
    public IWebSocketClient Client { get; }

    public RegisteredWebSocketClient(IDisposable registrationToken, IWebSocketClient client)
    {
        _registrationToken = registrationToken;
        Client = client;
    }

    public async ValueTask DisposeAsync()
    {
        _registrationToken.Dispose();
        await Client.DisposeAsync();
    }
}