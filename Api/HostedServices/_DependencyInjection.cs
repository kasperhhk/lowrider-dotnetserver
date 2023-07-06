namespace Api.HostedServices;

public static class DependencyInjection
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        => services
            .AddHostedService<ChatBotHostedService>();
}
