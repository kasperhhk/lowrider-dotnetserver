namespace Api.Features.WebSockets;

public static class WebSocketsFeature {
  public static IServiceCollection AddWebSocketsFeature(this IServiceCollection serviceCollection)
    => serviceCollection
      .AddSingleton<IConnectionManager, ConnectionManager>()
      .AddSingleton<WebSocketConnectionFactory>()
      .AddSingleton<IMessageSerializer, JsonChatMessageSerializer>();
}