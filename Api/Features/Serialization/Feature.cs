using Api.Features.Serialization.Json;
using K;

namespace Api.Features.WebSockets;

public static class SerializationFeature {
  public static IServiceCollection AddSerializationFeature(this IServiceCollection serviceCollection)
    => serviceCollection.AddAllImplementingTypes(typeof(IJsonSerialization));
}