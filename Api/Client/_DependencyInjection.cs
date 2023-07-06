using Api.Client.WebSockets;

namespace Api.Client;

public static class DependencyInjection
{
    public static IServiceCollection AddPackageClient(this IServiceCollection services)
        => services
            .AddSingleton<IClientManager, ClientManager>()
            .AddWebSocketClient();
}
