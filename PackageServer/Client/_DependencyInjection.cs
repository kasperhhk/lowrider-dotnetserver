using Api.Client.InMemory;
using Api.Client.WebSockets;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Client;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPackageClient(this IServiceCollection services)
        => services
            .AddSingleton<IClientManager, ClientManager>()
            .AddWebSocketClient()
            .AddInMemoryClient();
}
