using Microsoft.Extensions.DependencyInjection;

namespace Api.Messaging.Recipients;

internal static class DependencyInjection
{
    internal static IServiceCollection AddRecipients(this IServiceCollection services)
        => services
            .AddSingleton<IRecipientsResolver, RecipientsResolver>();
}
