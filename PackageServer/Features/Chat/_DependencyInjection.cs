using Api.Features.Chat.Handlers;
using Api.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Features.Chat;

internal static class DependencyInjection
{
    internal static IServiceCollection AddChatFeature(this IServiceCollection services)
        => services
            .AddSingleton<IncomingChatMessageHandler>()
            .AddSingleton<IFeatureHandler, ChatFeatureHandler>();
}
