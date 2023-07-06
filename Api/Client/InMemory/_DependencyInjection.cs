using Api.Messaging;

namespace Api.Client.InMemory;

public static class DependencyInjection
{
    public static IServiceCollection AddInMemoryClient(this IServiceCollection services)
        => services
            .AddSingleton<IInMemoryClientFactory, InMemoryClientFactory>()
            .AddSingleton<IClientSender, InMemoryClientSender>();
}
