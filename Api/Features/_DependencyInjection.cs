using Api.Features.Chat;

namespace Api.Features;

public static class DependencyInjection
{
    public static IServiceCollection AddPackageFeatures(this IServiceCollection services)
        => services
            .AddChatFeature();
}
