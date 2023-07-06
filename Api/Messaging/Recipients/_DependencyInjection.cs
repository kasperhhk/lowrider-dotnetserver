namespace Api.Messaging.Recipients;

public static class DependencyInjection
{
    public static IServiceCollection AddRecipients(this IServiceCollection services)
        => services
            .AddSingleton<IRecipientsResolver, RecipientsResolver>();
}
