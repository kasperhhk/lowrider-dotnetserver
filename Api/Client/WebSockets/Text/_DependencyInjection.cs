using Api.Messaging;

namespace Api.Client.WebSockets.Text;

public static class _DependencyInjection
{
    public static IServiceCollection AddTextWebSocketClient(this IServiceCollection services)
        => services
            .AddSingleton<IWebSocketClientFactory, TextWebSocketClientFactory>()
            .AddSingleton<IClientSender, TextWebSocketClientSender>();
}
