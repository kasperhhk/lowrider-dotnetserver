using Api.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Client.InMemory;

internal static class DependencyInjection
{
    internal static IServiceCollection AddInMemoryClient(this IServiceCollection services)
        => services
            .AddSingleton<IInMemoryClientFactory, InMemoryClientFactory>()
            .AddSingleton<IClientSender, InMemoryClientSender>();
}
