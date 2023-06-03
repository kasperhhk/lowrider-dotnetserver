using Api.Features.WebSockets;

namespace K;

public static class UsersFeature
{
  public static IServiceCollection AddUsersFeature(this IServiceCollection serviceCollection)
    => serviceCollection
      .AddSingleton<IUserManager, UserManager>();
}