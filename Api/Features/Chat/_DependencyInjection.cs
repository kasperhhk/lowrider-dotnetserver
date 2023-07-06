using Api.Features.Chat.Handlers;
using Api.Messaging;

namespace Api.Features.Chat;

public static class DependencyInjection
{
    public static IServiceCollection AddChatFeature(this IServiceCollection services)
        => services
            .AddSingleton<IncomingChatMessageHandler>()
            .AddSingleton<IFeatureHandler, ChatFeatureHandler>();
}
