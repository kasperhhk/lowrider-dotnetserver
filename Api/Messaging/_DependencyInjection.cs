using Api.Messaging.Recipients;

namespace Api.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddPackageMessaging(this IServiceCollection services)
        => services
            .AddSingleton<IMessageBroker, MessageBroker>()
            .AddRecipients();
}
