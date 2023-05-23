using Api.Features.WebSockets;

public static class Features {
  public static IServiceCollection AddAllFeatures(this IServiceCollection serviceCollection)
    => serviceCollection
      .AddWebSocketsFeature();
}