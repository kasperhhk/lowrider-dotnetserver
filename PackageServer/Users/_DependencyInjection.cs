using Microsoft.Extensions.DependencyInjection;

namespace Api.Users;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPackageUsers(this IServiceCollection services)
        => services
            .AddSingleton<IUserManager, UserManager>();
}
