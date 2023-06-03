using K;

namespace Api.Features.WebSockets;

public static class WebSocketsFeature {
  public static IServiceCollection AddWebSocketsFeature(this IServiceCollection serviceCollection)
    => serviceCollection
      .AddSingleton<IOutboundMessageCentral, OutboundWebSocketMessageCentral>()
      .AddSingleton<IInboundMessageCentral, InboundWebSocketMessageCentral>()
      .AddSingleton<IWebSocketClientFactory, WebSocketClientFactory>()
      .AddSingleton<IWebSocketSerializationStrategy, JsonWebSocketSerializationStrategy>();
}