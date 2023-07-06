using Api.Client.WebSockets.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Client.WebSockets;

internal static class DepdencyInjection
{
    internal static IServiceCollection AddWebSocketClient(this IServiceCollection services)
        => services
            .AddTextWebSocketClient();
}
