using Api.Features.WebSockets;

namespace K;

public static class Features
{
  public static IServiceCollection AddAllFeatures(this IServiceCollection serviceCollection)
    => serviceCollection
      .AddWebSocketsFeature()
      .AddUsersFeature()
      .AddSerializationFeature()
      .AddMessagingFeature()
      .AddChatFeature();
}