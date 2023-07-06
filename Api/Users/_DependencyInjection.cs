namespace Api.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddPackageUsers(this IServiceCollection services)
        => services
            .AddSingleton<IUserManager, UserManager>();
}
