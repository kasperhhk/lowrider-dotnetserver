using Api.Features.Chat;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Features;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPackageFeatures(this IServiceCollection services)
        => services
            .AddChatFeature();
}
