using System.Reflection;

namespace K;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddAllImplementingTypes(this IServiceCollection serviceCollection, Type interfaceType)
  {
    var matches = Assembly.GetAssembly(interfaceType)!
      .ExportedTypes
      .Where(t => t is {IsAbstract: false, IsClass: true } && interfaceType.IsAssignableFrom(t))
      .Select(t => ServiceDescriptor.Singleton(interfaceType, t))
      .ToList();

    foreach (var descriptor in matches) {
      serviceCollection.Add(descriptor);
    }

    return serviceCollection;
  }
}