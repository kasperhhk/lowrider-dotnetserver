namespace Api.Serialization.Json;

public static class DependencyInjection
{
    public static IServiceCollection AddJsonPackageSerialization(this IServiceCollection services)
        => services
            .AddSingleton<IPackageSerializer, JsonPackageSerializer>();
}
