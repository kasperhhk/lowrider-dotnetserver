using Api.Features.WebSockets;

namespace K;

public static class MessagingFeature
{
  public static IServiceCollection AddMessagingFeature(this IServiceCollection serviceCollection)
    => serviceCollection
      .AddSingleton<IClientManager, ClientManager>()
      .AddAllImplementingTypes(typeof(IMessageHandler));
}