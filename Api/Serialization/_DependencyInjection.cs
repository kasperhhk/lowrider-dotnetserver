using Api.Serialization.Json;

namespace Api.Serialization;

public static class DependencyInjection
{
    public static IServiceCollection AddPackageSerialization(this IServiceCollection services)
        => services
            .AddJsonPackageSerialization();
}
