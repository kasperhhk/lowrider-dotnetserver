﻿using Api.Client;
using Api.Features;
using Api.Messaging;
using Api.Serialization;
using Api.Users;

namespace Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPackageServer(this IServiceCollection services)
        => services
            .AddPackageClient()
            .AddPackageFeatures()
            .AddPackageMessaging()
            .AddPackageSerialization()
            .AddPackageUsers();
}
