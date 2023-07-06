using Microsoft.Extensions.DependencyInjection;

namespace Api.Serialization.Json;

internal static class DependencyInjection
{
    internal static IServiceCollection AddJsonPackageSerialization(this IServiceCollection services)
        => services
            .AddSingleton<IJsonPackageSerializer, JsonPackageSerializer>();
}
