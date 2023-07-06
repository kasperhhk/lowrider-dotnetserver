using Api.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Client.WebSockets.Text;

internal static class DependencyInjection
{
    internal static IServiceCollection AddTextWebSocketClient(this IServiceCollection services)
        => services
            .AddSingleton<IWebSocketClientFactory, TextWebSocketClientFactory>()
            .AddSingleton<IClientSender, TextWebSocketClientSender>();
}
