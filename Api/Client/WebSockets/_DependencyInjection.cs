namespace Api.Client.WebSockets;

public static class DepdencyInjection
{
    public static IServiceCollection AddWebSocketClient(this IServiceCollection services)
        => services
            .AddSingleton<IWebSocketClientFactory, TextWebSocketClientFactory>();
}
