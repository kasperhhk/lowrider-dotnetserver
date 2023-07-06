using Api.Client.WebSockets.Text;

namespace Api.Client.WebSockets;

public static class DepdencyInjection
{
    public static IServiceCollection AddWebSocketClient(this IServiceCollection services)
        => services
            .AddTextWebSocketClient();
}
