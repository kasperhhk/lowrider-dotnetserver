using Api.Messaging.Recipients;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Messaging;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPackageMessaging(this IServiceCollection services)
        => services
            .AddSingleton<IMessageBroker, MessageBroker>()
            .AddRecipients();
}
