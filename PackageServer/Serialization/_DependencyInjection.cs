using Api.Serialization.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Serialization;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPackageSerialization(this IServiceCollection services)
        => services
            .AddJsonPackageSerialization();
}
